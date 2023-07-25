import { createClient, startCacheServer } from "../index";

startCacheServer("./test-files/");
// startCacheServer("F:/Work/packages/rttist/dev/hookyns/transformer/src");


const client = createClient();

// Warmup
client.getFile("1/index.ts");


client.getFile("1/index2.ts");
client.getFile("1/index.ts");
client.getFile("1/index.ts");
client.getFile("1/index.ts");
client.getFile("1/index.ts");
client.getFile("1/index.ts");
client.getFile("1/index.ts");
client.getFile("1/index.ts");
client.getFile("1/index.ts");
client.getFile("1/index.ts");

// setTimeout(() => {
(async() => {
	await client.getFileAsync("1/index.ts");
	const start = performance.now();
	await client.getFileAsync("1/index.ts");
	await client.getFileAsync("1/index.ts");
	client.getFile("1/index.ts");
	client.getFile("1/index.ts");
	client.getFile("1/index.ts");
	client.getFile("1/index.ts");
	client.getFile("1/index.ts");
	client.getFile("1/index.ts");
	client.getFile("1/index.ts");
	client.getFile("1/index.ts");
	await client.getFileAsync("1/index.ts");
	await client.getFileAsync("1/index.ts");
	await client.getFileAsync("1/index.ts");
	await client.getFileAsync("1/index.ts");
	const end = performance.now();

	console.log(`Time: ${end - start} ms`);

	client.dispose();
})();