# Morse Code Translator

This is a command-line interface (CLI) application that translates text to Morse code and vice versa. The project is part of the `MorseCode.UI.ConsoleApp` in the solution, which is the main entry point for running the translator.

## Features
- Encode text into Morse code.
- Decode Morse code back into text.
- Configurable file paths for input, output, and Morse code specification.
- Adjustable log levels to control the verbosity of the output.
- Easy-to-use command-line interface with helpful options.

## Usage

To use the Morse Code Translator, run the executable from the command line with the appropriate options.

```bash
MorseCode [options]
```

### Options

- `-cp, --code-path`
  - **Description**: Specify the path to the JSON file containing the Morse code specification.
  - **Default**: `morse-code-spec.json`

- `-rp, --read-path`
  - **Description**: Specify the path to the input file to encode or decode.
  - **Default**: `morse-code-encode-input.txt`

- `-wp, --write-path`
  - **Description**: Specify the path to the output file where the results will be written.
  - **Default**: `morse-code-output.txt`

- `-d, --direction`
  - **Description**: Specify whether to encode text to Morse code or decode Morse code to text.
  - **Options**: `encode`, `decode`
  - **Default**: `encode`

- `-ll, --log-level`
  - **Description**: Set the log level for the application.
  - **Options**: `INFO`, `DEBUG`, `TRACE`
  - **Default**: `INFO`

- `-h, --help`
  - **Description**: Show the help menu and exit.

### Example Commands

1. **Encoding Text to Morse Code:**

```bash
MorseCode -d encode -rp morse-code-encode-input.txt -wp morse-code-output.txt
```

2. **Decoding Morse Code to Text:**

```bash
MorseCode -d decode -rp morse-code-decode-input.txt -wp morse-code-output.txt
```

3. **Specifying a Custom Morse Code Specification File:**

```bash
MorseCode -cp custom-morse-code-spec.json
```

4. **Adjusting Log Level to Debug:**

```bash
MorseCode -ll DEBUG
```

## Morse Code Specification

The Morse code translation is driven by a specification file in JSON format. By default, the program uses the file `morse-code-spec.json`, which is located in the `MorseCode.UI.ConsoleApp` project.

You can provide a custom specification file using the `-cp` or `--code-path` option if needed. The JSON file should map characters to their respective Morse code representations.

### Example `morse-code-spec.json` Structure

```json
{
    "A": [".","-"],
    "B": ["-",".",".","."],
    "C": ["-",".","-","."],
    "D": ["-",".","."],
    "E": ["."],
    ...
}
```

## Sample Files

Two sample files are included in the project for testing the encoding and decoding functionality:

1. `morse-code-encode-input.txt`: A text file with plain text that can be encoded into Morse code.
2. `morse-code-decode-input.txt`: A text file with Morse code that can be decoded back into plain text.

You can use these files with the CLI to test the functionality of the program.

## Build and Run

### Prerequisites

- .NET 6.0 SDK

### Building the Project

Navigate to the solution directory and run the following command to build the project:

```bash
dotnet build
```

### Running the CLI

After building, you can run the CLI directly using the following command:

```bash
dotnet run --project MorseCode.UI.ConsoleApp -- [options]
```

Replace `[options]` with the appropriate command-line arguments.

### Running Tests

If you have unit tests set up for the project, you can run them with the following command:

```bash
dotnet test
```
