import { getAddonPath } from "./get-addon-path";

export type Client = {
	getFileAsync(fileName: string): Promise<string>;
	getFile(fileName: string): string | undefined;
	dispose(): void;
};

type Module = {
	startCacheServer(workingDirectory: string, tcpPort: number): void;
	stopCacheServer(): void;
	createClient(tcpPort: number): Client;
};

// Import the native addon
const { Module } = require(getAddonPath()) as { Module: Module };

/**
 * Start cache server in the background.
 * @param workingDirectory
 * @param tcpPort
 */
export function startCacheServer(workingDirectory: string, tcpPort: number = 9876) {
	Module.startCacheServer(workingDirectory, tcpPort);
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
	return Module.createClient(tcpPort);
}