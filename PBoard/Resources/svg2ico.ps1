# Require PowerShell 7+ (для лучшего управления консолью)
# Запускать: pwsh .\script.ps1

$ErrorActionPreference = "Stop"

# Проверка наличия ImageMagick
if (-not (Get-Command magick -ErrorAction SilentlyContinue)) {
    Write-Host "ImageMagick не установлен!" -ForegroundColor Red
    exit 1
}

# Поиск файлов
$files = @(Get-ChildItem -Filter *.svg)
if ($files.Count -eq 0) {
    Write-Host "SVG файлы не найдены" -ForegroundColor Yellow
    exit
}

# Простой прогресс-бар
function Show-Progress {
    param($current, $total)
    $percent = $current / $total * 100
    $progress = [math]::Round($percent)
    Write-Host "`rПрогресс: [" -NoNewline
    Write-Host ("#" * $progress) -ForegroundColor Cyan -NoNewline
    Write-Host (" " * (100 - $progress)) -NoNewline
    Write-Host "] $($progress)% ($current/$total)" -NoNewline
}

# Основной цикл
$success = 0
$errors = 0

Write-Host "Начало обработки $($files.Count) файлов..."

foreach ($i in 0..($files.Count-1)) {
    $file = $files[$i]
    $icoPath = Join-Path $file.DirectoryName "$($file.BaseName).ico"
    
    try {
        # Вывод текущего файла
        Write-Host "`nОбработка: $($file.Name)" -ForegroundColor Gray
        
        # Конвертация
        & magick -density 384 -background transparent $file.FullName -define icon:auto-resize $icoPath
        
        # Статус
        Write-Host "Успешно: $icoPath" -ForegroundColor Green
        $success++
    }
    catch {
        Write-Host "Ошибка: $($_.Exception.Message)" -ForegroundColor Red
        $errors++
    }
    
    # Обновление прогресс-бара
    Show-Progress ($i+1) $files.Count
}

# Итог
Write-Host "`n`nГотово! Успешно: $success, Ошибок: $errors" -ForegroundColor Cyan