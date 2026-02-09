# Elegoo RFID Tag Editor - Windows Desktop

A Windows desktop application for editing Elegoo NTAG213 NFC filament spool tags. Manage your 3D printer filament spool RFID tags with an intuitive WinForms interface.

🌐 **Web Version:** [https://savion.github.io/elegoo-rfid-editor/](https://savion.github.io/elegoo-rfid-editor/)

## Features

✨ **Native Windows Application** - Fast, responsive WinForms interface
📊 **Dual Editor Interface** - Grid view and form controls, auto-synced
🎨 **Color-Coded Rows** - Visual indicators for different field types
✅ **Input Validation** - Real-time validation with color feedback (green=valid, red=invalid)
🔍 **Field Usage Indicators** - ✓ = Used by printer, ⓘ = Metadata only
📅 **Quick Actions** - Set current date, fix checksums, generate new tags
💾 **Complete NTAG213 Support** - Full 180-byte memory map

## Supported Features

### Active Fields (Read by Printer) ✓
- **Material** - Select from 15 supported materials (PLA, PETG, ABS, TPU, PA, CPE, PC, PVA, ASA, BVOH, EVA, HIPS, PP, PPA, PPS)
- **Supplement** - Choose from 52 material subtypes (PLA-CF, PETG-GF, TPU 95A, etc.)
- **Filament Color** - RGB color picker with hex input

### Metadata Fields ⓘ
These fields are stored but not currently used by the printer:
- Weight (grams)
- Diameter (mm × 100)
- Temperature Range (Min/Max in °C)
- Production Date (YYMM format)
- Color Modifier (L/M/D)

## System Requirements

- Windows 10/11
- .NET 8.0 Runtime
- 10 MB disk space

## Quick Start

### For Users

1. Download the latest release from the [Releases](https://github.com/Savion/elegoo-rfid-editor-desktop/releases) page
2. Extract the ZIP file
3. Run `NFCEditor.exe`
4. Click **"Generate New"** to create a blank tag, or **"Load .BIN"** to edit an existing one
5. Select your **Material** and **Supplement**
6. Pick your **Filament Color**
7. Optionally fill in metadata fields
8. Click **"Fix Checksum"** to ensure the tag is valid
9. **Save .BIN** to your device

### For Developers

#### Prerequisites

- Visual Studio 2022 or later
- .NET 8.0 SDK
- Windows 10/11

#### Build Instructions

```bash
# Clone the repository
git clone https://github.com/Savion/elegoo-rfid-editor-desktop.git
cd elegoo-rfid-editor-desktop

# Open in Visual Studio
start RFIDEditor.csproj

# Or build from command line
dotnet build
dotnet run
```

#### Publish

```bash
# Publish self-contained executable
dotnet publish -c Release -r win-x64 --self-contained
```

The output will be in `bin/Release/net8.0-windows/win-x64/publish/`

## Project Structure

```
NFCEditor/
├── ElegooSpool.cs           # Core data model for NTAG213 structure
├── RFIDEditor.cs            # Main form logic
├── RFIDEditor.Designer.cs   # WinForms designer code
├── Program.cs               # Application entry point
├── CLAUDE.md                # Development documentation
└── QUICK_REFERENCE.md       # Quick reference guide
```

## How It Works

### Binary Data Processing

The application handles NTAG213 180-byte binary data:

```csharp
// Load a .BIN file
byte[] data = File.ReadAllBytes("tag.bin");
var spool = new ElegooSpool(data);

// Edit fields
spool.Material = "PLA";
spool.MaterialSubtype = "PLA-CF";
spool.FilamentColor = Color.FromArgb(255, 0, 0);

// Save
File.WriteAllBytes("tag.bin", spool.RawData);
```

### Material Encoding

Materials are encoded as 32-bit signatures:
- PLA = `0x00807665`
- PETG = `0x80698471`
- ABS = `0x00656683`
- etc.

Subtypes use 16-bit codes with family-based encoding:
- High byte = material family (0x00 = PLA, 0x01 = PETG, etc.)
- Low byte = variant (0x04 = Carbon Fiber, 0x02 = Glass Fiber, etc.)

### Checksums

The BCC1 checksum at byte 0x08 is calculated as:
```csharp
BCC1 = UID[4] ^ UID[5] ^ UID[6] ^ UID[7]
```

The application automatically recalculates this when you click "Fix Checksum".

## Visual Indicators

### Row Colors
- **Orange** - Material and supplement
- **Green** - Filament color (RGB)
- **Blue** - Temperature settings
- **Purple** - Production date

### Field Labels
- **✓** - Field is read and used by the printer
- **ⓘ** - Metadata field (stored but not currently used by printer)

### Input Validation
- **Green background** - Valid input
- **Red background** - Invalid input

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## Related Projects

- [Web Version](https://github.com/Savion/elegoo-rfid-editor) - Browser-based React/TypeScript application
- [RFID Tools](https://play.google.com/store/apps/details?id=com.wakdev.wdnfc) - Mobile NFC reader/writer app

## License

MIT License - feel free to use this project however you'd like!

## Support

If you find this tool useful, please ⭐ star the repository!

For issues or questions, please [open an issue](https://github.com/Savion/elegoo-rfid-editor-desktop/issues).

---

**Note:** This tool is for educational and personal use. Make sure you have permission before modifying RFID tags.
