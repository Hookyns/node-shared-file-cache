# Memory-Mapped Files
> Native `.node` addon for node.js written in C#.

Memory-mapped files offer a significant performance benefit over standard file access.
It reduces the number of times the application must access the disk (which is slow compared to memory).
In case you use thread workers, you can share the memory-mapped files between them, so you will also reduce the number of redundant file reads.

Native file reading is much faster than reading from a node.js.
Memory-mapped files are intended mainly for workers so it creates TCP server that holds the files; Workers transfer the files over the network so it slows it down a little, but it is still much faster than reading from a node.js.


## Start the Cache Server
You can start the cache server in the background using the `startCacheServer` function.
```typescript
import { startCacheServer } from "memory-mapped-files";

startCacheServer("./working/directory");
```

You can also stop the cache server using the `stopCacheServer` function.
```typescript
stopCacheServer();
```

Second parameter of the `startCacheServer` is an array of glob patterns that specify which files from the working directory should be cached.

```typescript
import { startCacheServer } from "memory-mapped-files";

startCacheServer("./working/directory", ["**/*.ts"]);
```

Default value glob is `**/*`, so all the files from working directory are cached.

### Tracking changes
Memory-mapped files does not track changes in the working directory. File deletions or changes are not reflected in the cache.
You can change the files using the client (not fully implemented yet).

## Read File
You can read the file using the `getFile` and `getFileAsync` methods on the `Client`. 
File paths should be relative from the working directory.

Each client creates a new TCP connection to the cache server, so it is recommended to create a single client and reuse it.
One client should not do parallel requests; multiple clients can.

```typescript
import { createClient } from "memory-mapped-files";

const client = createClient();

// Sync read
client.getFile("some-dir/index.ts");

// Async read
await client.getFileAsync("some-dir/index.ts");

client.dispose();
```

### Disposing Client
You should call the dispose method when you are done with the client.
You can do it manually or use the new `using` keyword from TS 5.2, see the [release note](https://devblogs.microsoft.com/typescript/announcing-typescript-5-2-beta/#using-declarations-and-explicit-resource-management).

```typescript
import { createClient } from "memory-mapped-files";

const client = createClient();
client.getFile("some-dir/index.ts");
client.dispose();
```
or
```typescript
import { createClient } from "memory-mapped-files";

using client = createClient();
client.getFile("some-dir/index.ts");
```