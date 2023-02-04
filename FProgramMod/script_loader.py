import os
import sys

def RunScript(fname):
    filename = os.path.join(os.getcwd(), "scripts", fname)
    with open(filename, "rb") as source_file:
        code = compile(source_file.read(), filename, "exec")
    return f"Script {fname} loaded."
    exec(code)