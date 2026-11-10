# ImageViewer — Agent Guide

## Stack
- .NET 10 Windows Forms (`net10.0-windows`)
- Single project in `ImageViewer/ImageViewer.csproj`
- Entrypoint: `ImageViewer/Program.cs` → `Form1`

## Commands
```powershell
dotnet build .\ImageViewer.sln
dotnet build -c Release .\ImageViewer.sln
dotnet run --project .\ImageViewer\ImageViewer.csproj
```

## Architecture
- `Form1.cs` — main form with all keyboard-driven logic (no DI, no abstractions)
- `Form1.Designer.cs` — designer-generated UI (do not edit manually)
- `Services/FileService.cs` — empty stub, unused
- `empty.png` — fallback image shown when no files remain (copied to output)
- No tests, no CI, no package dependencies beyond `Microsoft.NET.Sdk` + WinForms

## Key bindings (from code)
| Key | Action |
|---|---|
| `Open folder` button | Pick folder → load files → random image → fullscreen |
| `F` | Toggle fullscreen |
| `↑` / `↓` | Random image / first image |
| `→` / `←` | Next / previous |
| `Delete` / `D` | → `deleted/` (restorable via stack, one level) |
| `Backspace` | Restore last deleted |
| `PageUp` / `S` | → `saved/` |
| `]` (`Ъ` in RU layout) | Rotate current image 90° clockwise, overwrite same file |
| `A` | → `achtung/` |
| `Esc` | Close app |

- On close, `deleted/` is permanently purged.
- Only the last delete is restorable.
- Folder loading keeps only image files (`jpg/jpeg/png/bmp/gif/tif/tiff/ico`); other files are ignored.
- If a folder has no images, the overlay shows `Изображения не найдены` instead of the file counter.