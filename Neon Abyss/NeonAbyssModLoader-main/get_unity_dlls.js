const fs = require("fs");

const dllsToGet = [];

const original_stripped_path = "./game_managed_dll/"
const unstripped_path = "./unity2018_assemblies/"
const outunstripped_path = "./out/"
const original_files = fs.readdirSync(original_stripped_path);

let copied = [];
original_files.forEach(element => {
    const out_path = outunstripped_path + element;
    fs.copyFileSync(original_stripped_path + element, out_path)

    const path = unstripped_path + element;
    if (fs.existsSync(path)) {
        copied.push(element);
        fs.copyFileSync(path, out_path)
    }
});
console.log(`${copied.length}/${original_files.length} copied`)
copied.forEach(x => console.log("-->", x))