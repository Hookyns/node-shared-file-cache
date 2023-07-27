// @ts-ignore
import {createClient, startCacheServer} from "../../dist/index";
import * as fs from "node:fs";
import * as path from "path";

const testFilesBasePath  = path.resolve(__dirname, "..", "test-files");

startCacheServer("test-files/");
// startCacheServer("F:/Work/packages/rttist/dev/hookyns/transformer/src");


const client = createClient();


// Warmup
client.getFile("2/index.ts");
fs.readFileSync(path.join(testFilesBasePath, "1/index.ts"), "utf-8");

const start = performance.now();
for (let i = 0; i < 1000; i++) {
    client.getFile("2/index.ts");
    client.getFile("2/helpers.ts");
    client.getFile("2/Metadata.ts");
    client.getFile("2/Module.ts");
    client.getFile("2/Reflect.ts");
    client.getFile("3/index.ts");
    client.getFile("3/helpers.ts");
    client.getFile("3/Metadata.ts");
    client.getFile("3/Module.ts");
    client.getFile("3/Reflect.ts");
}
const end = performance.now();
console.log(`Time: ${end - start} ms (Memory-Mapped Files)`);


const startFs = performance.now();
for (let i = 0; i < 1000; i++) {
    fs.readFileSync(path.join(testFilesBasePath, "2/index.ts"), "utf-8");
    fs.readFileSync(path.join(testFilesBasePath, "2/helpers.ts"), "utf-8");
    fs.readFileSync(path.join(testFilesBasePath, "2/Metadata.ts"), "utf-8");
    fs.readFileSync(path.join(testFilesBasePath, "2/Module.ts"), "utf-8");
    fs.readFileSync(path.join(testFilesBasePath, "2/Reflect.ts"), "utf-8");
    fs.readFileSync(path.join(testFilesBasePath, "3/index.ts"), "utf-8");
    fs.readFileSync(path.join(testFilesBasePath, "3/helpers.ts"), "utf-8");
    fs.readFileSync(path.join(testFilesBasePath, "3/Metadata.ts"), "utf-8");
    fs.readFileSync(path.join(testFilesBasePath, "3/Module.ts"), "utf-8");
    fs.readFileSync(path.join(testFilesBasePath, "3/Reflect.ts"), "utf-8");
}
const endFs = performance.now();
console.log(`Time: ${endFs - startFs} ms (node.js fs.readFileSync)`);
// (async() => {
// 	const startFs = performance.now();
// 	await fsAsync.readFile(path.resolve(__dirname, "test-files/2/index.ts"), "utf-8");
// 	await fsAsync.readFile(path.resolve(__dirname, "test-files/2/helpers.ts"), "utf-8");
// 	await fsAsync.readFile(path.resolve(__dirname, "test-files/2/Metadata.ts"), "utf-8");
// 	await fsAsync.readFile(path.resolve(__dirname, "test-files/2/Module.ts"), "utf-8");
// 	await fsAsync.readFile(path.resolve(__dirname, "test-files/2/Reflect.ts"), "utf-8");
// 	await fsAsync.readFile(path.resolve(__dirname, "test-files/3/index.ts"), "utf-8");
// 	await fsAsync.readFile(path.resolve(__dirname, "test-files/3/helpers.ts"), "utf-8");
// 	await fsAsync.readFile(path.resolve(__dirname, "test-files/3/Metadata.ts"), "utf-8");
// 	await fsAsync.readFile(path.resolve(__dirname, "test-files/3/Module.ts"), "utf-8");
// 	await fsAsync.readFile(path.resolve(__dirname, "test-files/3/Reflect.ts"), "utf-8");
// 	const endFs = performance.now();
// 	console.log(`Time: ${endFs - startFs} ms (node.js fs.readFile)`);
// })();

//
// // setTimeout(() => {
// (async() => {
// 	const start = performance.now();
// 	await client.getFileAsync("1/index.ts");
// 	await client.getFileAsync("1/index.ts");
// 	await client.getFileAsync("1/index.ts");
// 	client.getFile("1/index.ts");
// 	client.getFile("1/index.ts");
// 	client.getFile("1/index.ts");
// 	client.getFile("1/index.ts");
// 	client.getFile("1/index.ts");
// 	client.getFile("1/index.ts");
// 	client.getFile("1/index.ts");
// 	client.getFile("1/index.ts");
// 	await client.getFileAsync("1/index.ts");
// 	await client.getFileAsync("1/index.ts");
// 	await client.getFileAsync("1/index.ts");
// 	await client.getFileAsync("1/index.ts");
// 	const end = performance.now();
//
// 	console.log(`Time: ${end - start} ms`);
// })();

setTimeout(() => {
    client.dispose();
}, 100);