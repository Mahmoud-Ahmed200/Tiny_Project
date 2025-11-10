
## Tiny Language Compiler

###  Overview

This project is a **compiler for the Tiny programming language**, implemented in **C#**.
It performs the main **compiler phases** including:

* Lexical Analysis (Scanner)
* Syntax Analysis (Parser)

The Tiny language supports variables, expressions, conditionals, loops, I/O operations, and functions.

---

###  Features

**Lexical Analysis (Scanner Phase)**

* Reads the source code and tokenizes it into valid **lexemes**.
* Supports:

  * Reserved words (`int`, `float`, `read`, `write`, `repeat`, `until`, `if`, `elseif`, `else`, `then`, `return`, `endl`)
  * Identifiers
  * Numbers (integers and reals)
  * Operators (`+`, `-`, `*`, `/`, `=`, `<`, `>`, `<=`, `>=`, `==`, `<>`)
  * Punctuation (`;`, `,`, `{`, `}`, `(`, `)`)
  * Strings and special symbols

**Syntax Analysis (Parser Phase)**

* Uses a **recursive descent parser** to check if the token stream follows Tiny’s grammar rules.
* Displays **syntax errors** with line numbers and descriptive messages.
* Builds a **parse tree** if the source code is syntactically correct.

**Error Handling**

* Detects and reports:

  * **Lexical errors** (invalid characters, malformed numbers, unclosed strings)
  * **Syntax errors** (unexpected tokens, missing symbols, misplaced keywords)

---

### Project Structure

```
TinyCompiler/
│
├── Scanner/
│   ├── Scanner.cs              # Performs lexical analysis
│   ├── Token.cs                # Token structure definition
│   ├── RegexHelpers.cs         # Helper functions for token regex patterns
│
├── Parser/
│   ├── Parser.cs               # Implements recursive descent parser
│   ├── Grammar.txt             # Contains grammar rules for reference
│
├── Input/
│   └── sample.tiny             # Example Tiny program
│
├── Output/
│   ├── Tokens.txt              # List of generated tokens
│   ├── Errors.txt              # List of lexical/syntax errors
│
├── Program.cs                  # Entry point
└── README.md                   # Documentation
```

---

###  How to Run

####  Requirements

* **.NET SDK** (version 6.0 or higher)
* Any C# IDE (Visual Studio / Visual Studio Code)

####  Steps

1. Clone or download the repository:

   ```bash
   git clone https://github.com/Mahmoud-Ahmed200/Tiny_Project
   ```
2. Open the project in **Visual Studio**.
3. Run the program.
4. Enter the TINY code
5. Check:

   * **Lixemes** → `Lixemes`
   * **Tokens** → `Tokens`
   * **Errors** → `Errors`

---

###  Example Input

```tiny
int val, counter;
read val;
counter := 0;
repeat
    val := val - 1;
    write "Iteration number [";
    write counter;
    write "] the value of x = ";
    write val;
    write endl;
    counter := counter + 1;
until val = 0;
```

### Example Output (Tokens)

```
int -> Int
val -> Identifier
, -> Comma
counter -> Identifier
; -> Semicolon
read -> Read
val -> Identifier
; -> Semicolon
...
```

---

###  Tools Used

* **Language:** C# (.NET)
* **Editor:** Visual Studio 2022 / VS Code
* **Concepts Applied:**

  * Regular Expressions
  * DFA construction
  * Recursive Descent Parsing
  * Error Detection & Handling

---

