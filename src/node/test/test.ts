import { createClient, startCacheServer } from "../index";

startCacheServer("F:/Work/packages/rttist/dev/hookyns/transformer/src");


const client = createClient();

// setTimeout(() => {
(async() => {
	await client.getFileAsync("index.ts");
	const start = performance.now();
	await client.getFileAsync("index.ts");
	await client.getFileAsync("index.ts");
	client.getFile("index.ts");
	client.getFile("index.ts");
	client.getFile("index.ts");
	client.getFile("index.ts");
	client.getFile("index.ts");
	client.getFile("index.ts");
	client.getFile("index.ts");
	client.getFile("index.ts");
	await client.getFileAsync("index.ts");
	await client.getFileAsync("index.ts");
	await client.getFileAsync("index.ts");
	await client.getFileAsync("index.ts");
	const end = performance.now();

	console.log(`Time: ${end - start} ms`);

	client.dispose();
})();