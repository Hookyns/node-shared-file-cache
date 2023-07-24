import { dotnet } from './dotnet.js'

const { setModuleImports, getAssemblyExports, getConfig } = await dotnet
    .withDiagnosticTracing(false)
    .create();

setModuleImports('main.mjs', {
    nodeVersion: () => globalThis.process.version
    // node: {
    //     process: {
    //         version: () => globalThis.process.version
    //     }
    // }
});

const config = getConfig();
const exports = await getAssemblyExports(config.mainAssemblyName);

const { MemoryMappedCache } = exports.RJDev.MemoryMappedCache;

MemoryMappedCache.Start("F:/Work/packages/rttist/dev/hookyns/transformer/src", 9999);

await dotnet.run();
