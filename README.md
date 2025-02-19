# Hermes
 This language allows for basic programming features such as variable creation, functions, conditionals, modules, and input/output. You can define and import modules, use math functions (currently from C#), and perform basic operations like arithmetic and string concatenation.

### Usage

 You can run Hermes in interactive mode (REPL) using the `-i` flag:
 ```sh
 Hermes.exe -i
 ```

 or file mode using `-f <file_path>`:
 ```sh
 Hermes.exe -f <file_path>
 ```

# Key Features
 - Modules:
    - Modules can be created in the src/modules/ folder (or a folder specified in the config).
    - To import a module, use @import module_name.
    - Each module starts with !module["name"] and functions within it can be accessed in your main.hs file.


# Example Usage
 1. Input/Output:
    - Input:
        ```hs
        @import io
        var name = io.input();
        ```
    - Print:
        ```hs
        @import io
        io.print("str");
        io.print(1337);

        var h = 1337;
        io.print(h);

        var g = "str"
        io.print(g);
        ```
 2. Functions:
    - Define a function with func:
        ```hs
        func sum(a, b) {
            return a + b;
        }
        ```
    - Call the function:
        ```hs
        sum(3, 4);  // returns 7
        ```
 3. Conditionals:
    - Basic if, else, and else if statements:
        ```hs
        if (condition is true) { 
            // execute 
        } else if { // else if 
        } else { 
            // else 
        }
        ```
 4. String Concatenation:
    ```hs
    @import io
    io.print("str " + "1");

    var s = "s";
    io.print("s=" + s);
    ```

# Config
 The language allows you to configure your modules and customize the structure with a simple config file.