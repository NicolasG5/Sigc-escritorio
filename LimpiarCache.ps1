# Script de Limpieza Completa para Visual Studio
# Ejecutar como Administrador

Write-Host "================================================" -ForegroundColor Cyan
Write-Host "  Limpieza Completa de Visual Studio y Proyecto" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

# Paso 1: Cerrar Visual Studio
Write-Host "[1/6] Cerrando procesos de Visual Studio..." -ForegroundColor Yellow
Get-Process | Where-Object {$_.ProcessName -like "*devenv*" -or $_.ProcessName -like "*MSBuild*"} | ForEach-Object {
    Write-Host "  - Cerrando: $($_.ProcessName) (PID: $($_.Id))" -ForegroundColor Gray
    Stop-Process -Id $_.Id -Force -ErrorAction SilentlyContinue
}
Start-Sleep -Seconds 2
Write-Host "  ? Procesos cerrados" -ForegroundColor Green
Write-Host ""

# Paso 2: Limpiar carpetas del proyecto
Write-Host "[2/6] Limpiando carpetas del proyecto (bin, obj, .vs)..." -ForegroundColor Yellow
$projectPath = "C:\Duoc material de estudio\Login-In-WPF-MVVM-C-Sharp-and-SQL-Server-main2"
Set-Location $projectPath

$folders = @(
    "WPF-LoginForm\bin",
    "WPF-LoginForm\obj",
    ".vs"
)

foreach ($folder in $folders) {
    if (Test-Path $folder) {
        Write-Host "  - Eliminando: $folder" -ForegroundColor Gray
        Remove-Item -Path $folder -Recurse -Force -ErrorAction SilentlyContinue
    }
}
Write-Host "  ? Carpetas del proyecto limpiadas" -ForegroundColor Green
Write-Host ""

# Paso 3: Limpiar caché de Visual Studio del usuario
Write-Host "[3/6] Limpiando caché de Visual Studio..." -ForegroundColor Yellow
$vsCache = @(
    "$env:LOCALAPPDATA\Microsoft\VisualStudio\*\ComponentModelCache",
    "$env:LOCALAPPDATA\Microsoft\VisualStudio\*\*.tbd",
    "$env:TEMP\*.vshost.*"
)

foreach ($cache in $vsCache) {
    if (Test-Path $cache) {
        Write-Host "  - Limpiando: $cache" -ForegroundColor Gray
        Remove-Item -Path $cache -Recurse -Force -ErrorAction SilentlyContinue
    }
}
Write-Host "  ? Caché de Visual Studio limpiado" -ForegroundColor Green
Write-Host ""

# Paso 4: Ejecutar dotnet clean
Write-Host "[4/6] Ejecutando dotnet clean..." -ForegroundColor Yellow
Set-Location "$projectPath\WPF-LoginForm"
dotnet clean --verbosity quiet
Write-Host "  ? dotnet clean completado" -ForegroundColor Green
Write-Host ""

# Paso 5: Limpiar archivos temporales del sistema
Write-Host "[5/6] Limpiando archivos temporales del sistema..." -ForegroundColor Yellow
$tempFiles = @(
    "$env:TEMP\*.tmp",
    "$env:TEMP\*.cs",
    "$env:TEMP\.vs*"
)

foreach ($temp in $tempFiles) {
    Remove-Item -Path $temp -Force -ErrorAction SilentlyContinue
}
Write-Host "  ? Archivos temporales limpiados" -ForegroundColor Green
Write-Host ""

# Paso 6: Reconstruir proyecto
Write-Host "[6/6] Reconstruyendo proyecto..." -ForegroundColor Yellow
Set-Location $projectPath
msbuild "WPF-LoginForm\WPF-LoginForm.csproj" /t:Rebuild /p:Configuration=Debug /verbosity:minimal

if ($LASTEXITCODE -eq 0) {
    Write-Host "  ? Proyecto reconstruido exitosamente" -ForegroundColor Green
} else {
    Write-Host "  ? Error al reconstruir el proyecto" -ForegroundColor Red
}
Write-Host ""

# Resumen final
Write-Host "================================================" -ForegroundColor Cyan
Write-Host "  Limpieza Completada" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Siguiente paso:" -ForegroundColor Yellow
Write-Host "1. Abre Visual Studio" -ForegroundColor White
Write-Host "2. Abre la solución" -ForegroundColor White
Write-Host "3. Ve a Compilar ? Recompilar solución" -ForegroundColor White
Write-Host "4. Presiona F5 para ejecutar en modo Debug" -ForegroundColor White
Write-Host ""
Write-Host "Para verificar los cambios, abre la ventana de Salida (Ctrl+Alt+O)" -ForegroundColor Cyan
Write-Host "y selecciona 'Depurar' en el menú desplegable." -ForegroundColor Cyan
Write-Host ""

Read-Host "Presiona Enter para cerrar"
