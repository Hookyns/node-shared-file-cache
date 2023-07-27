import { resolve, join } from "path";
const { platform, arch } = process;

const addonsBasePath = resolve(__dirname, "..", "addons");

export function getAddonPath() {
	switch (platform) {
		case "win32":
			switch (arch) {
				case "x64":
					return join(addonsBasePath, "mmf.win-x64.node");
				// case 'ia32':
				//     return join(addonsBasePath, "mmf.win-ia32.node");
				// case 'arm64':
				//     return join(addonsBasePath, "mmf.win-arm64.node");
				default:
					throw new Error(`Unsupported Windows architecture: ${arch}`);
			}
		// case "darwin":
		// 	switch (arch) {
		// 		case 'x64':
		// 		    return join(addonsBasePath, "mmf.darvin-x64.node");
		// 		// case 'arm64':
		// 		//     return join(addonsBasePath, "mmf.darvin-arm64.node");
		// 		default:
		// 			throw new Error(`Unsupported MacOS architecture: ${arch}`);
		// 	}
		// case "freebsd":
		// 	switch (arch) {
		// 		case 'x64':
		// 		    return join(addonsBasePath, "mmf.darvin-x64.node");
		// 		default:
		// 			throw new Error(`Unsupported FreeBSD architecture: ${arch}`);
		// 	}
		case "linux":
			switch (arch) {
				case "x64":
					return join(addonsBasePath, "mmf.linux-x64.node");
				// case "arm64":
				// 	return join(addonsBasePath, "mmf.linux-arm64.node");
				default:
					throw new Error(`Unsupported Linux architecture: ${arch}`);
			}
		default:
			throw new Error(`Unsupported OS: ${platform}, architecture: ${arch}`);
	}
}
