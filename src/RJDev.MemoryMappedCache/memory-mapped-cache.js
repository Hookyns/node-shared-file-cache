import { readFile } from 'node:fs/promises';
import { resolve } from 'node:path';
import { WASI } from 'wasi';
import { argv, env } from 'node:process';

const wasm = await WebAssembly.compile(
    await readFile(new URL('./RJDev.MemoryMappedCache.wasm', import.meta.url)),
);

export async function start(workingDirectory, port) {
    const wasi = new WASI({
        version: 'preview1',
        args: [], //argv,
        env,
        preopens: {
            '/sandbox': resolve(process.cwd(), workingDirectory),
        },
    });

    debugger;
    const imports = wasi.getImportObject();

    const instance = await WebAssembly.instantiate(wasm, imports);

    wasi.start(instance);
}
