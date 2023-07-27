import * as path from "path";
import {getAddonPath} from "./get-addon-path";

export type Client = {
    getFileAsync(fileName: string): Promise<string>;
    getFile(fileName: string): string | undefined;
    dispose(): void;
    [Symbol.dispose](): void;
};

type Module = {
    startCacheServer(workingDirectory: string, include: string[], tcpPort: number): void;
    stopCacheServer(): void;
    createClient(tcpPort: number): Client;
};

// Import the native addon
const Module: Module = require(getAddonPath()).Module;

/**
 * Start cache server in the background.
 * @param workingDirectory Absolute path of the working directory or path relative CWD.
 * @param include Glob patterns to match files from working directory which should be cached.
 * @param tcpPort
 */
export function startCacheServer(workingDirectory: string, include: string[] = ["**/*"], tcpPort: number = 9876) {
    console.log("Working directory:", path.resolve(process.cwd(), workingDirectory));
    Module.startCacheServer(path.resolve(process.cwd(), workingDirectory), include, tcpPort);
}

/**
 * Stop cache server.
 */
export function stopCacheServer() {
    Module.stopCacheServer();
}

/**
 * Create client that can communicate with the cache server.
 * @param tcpPort
 */
export function createClient(tcpPort: number = 9876): Client {
    const client = Module.createClient(tcpPort);
    (client as any)[Symbol.dispose] = () => client.dispose();
    return client;
}