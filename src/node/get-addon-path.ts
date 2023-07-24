import { join } from "path";
const { platform, arch } = process;

export function getAddonPath() {
	switch (platform) {
		case "win32":
			switch (arch) {
				case "x64":
					return join(__dirname, "addons", "memory-mapped-cache.win-x64.node");
				// case 'ia32':
				//     return join(__dirname, "addons", "memory-mapped-cache.win-ia32.node");
				// case 'arm64':
				//     return join(__dirname, "addons", "memory-mapped-cache.win-arm64.node");
				default:
					throw new Error(`Unsupported Windows architecture: ${arch}`);
			}
		// case "darwin":
		// 	switch (arch) {
		// 		case 'x64':
		// 		    return join(__dirname, "addons", "memory-mapped-cache.darvin-x64.node");
		// 		// case 'arm64':
		// 		//     return join(__dirname, "addons", "memory-mapped-cache.darvin-arm64.node");
		// 		default:
		// 			throw new Error(`Unsupported MacOS architecture: ${arch}`);
		// 	}
		// case "freebsd":
		// 	switch (arch) {
		// 		case 'x64':
		// 		    return join(__dirname, "addons", "memory-mapped-cache.darvin-x64.node");
		// 		default:
		// 			throw new Error(`Unsupported FreeBSD architecture: ${arch}`);
		// 	}
		// case "linux":
		// 	switch (arch) {
		// 		case "x64":
		// 			return join(__dirname, "addons", "memory-mapped-cache.linux-x64.node");
		// 		case "arm64":
		// 			return join(__dirname, "addons", "memory-mapped-cache.linux-arm64.node");
		// 		default:
		// 			throw new Error(`Unsupported Linux architecture: ${arch}`);
		// 	}
		default:
			throw new Error(`Unsupported OS: ${platform}, architecture: ${arch}`);
	}
}
