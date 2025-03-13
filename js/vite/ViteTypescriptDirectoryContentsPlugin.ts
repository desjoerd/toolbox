import fs from "fs/promises";
import path from "path";
import { Plugin } from "vite";

async function getFilesRecursively(dir: string): Promise<string[]> {
  let results: string[] = [];

  const list = await fs.readdir(dir);

  await Promise.all(
    // we map the list of files to an array of promises so we can await them all
    // the map function is a bit dirty as it's not pure, and has a side effect of pushing to the results array
    list.map(async (file) => {
      const filePath = path.join(dir, file);
      const stat = await fs.stat(filePath);

      if (stat && stat.isDirectory()) {
        // Recurse into a subdirectory
        results.push(...(await getFilesRecursively(filePath)));
      } else {
        // Is a file
        results.push(filePath);
      }
    })
  );

  return results;
}

async function generateFileTypes(
  directoryPath: string,
  outputPath: string,
  typeName: string
) {
  try {
    // Get all file paths relative to the directoryPath
    const filePaths = (await getFilesRecursively(directoryPath))
      .map((file) => path.relative(directoryPath, file).replace(/\\/g, "/"))
      .sort((a, b) => a.localeCompare(b, undefined, { numeric: true }));

    // Create the TypeScript type definition
    const typeDefinition = `export type ${typeName} = ${filePaths
      .map((name) => `\n  | "${name}"`)
      .join("")};\n`;

    // Ensure the output directory exists
    await fs.mkdir(path.dirname(outputPath), { recursive: true });

    // Write the type definition to a .d.ts file
    await fs.writeFile(outputPath, typeDefinition);

    console.log(`Generated TypeScript types at ${outputPath}`);
  } catch (error) {
    console.error(error);
  }
}

export function generateFileTypesPlugin(options: {
  dir: string;
  output: string;
  typeName?: string;
}): Plugin {
  return {
    name: "generate-file-types",

    async buildStart() {
      const directoryPath = path.resolve(options.dir);
      const outputPath = path.resolve(options.output);
      await generateFileTypes(
        directoryPath,
        outputPath,
        options.typeName || "Files"
      );
    },

    async handleHotUpdate({ file }) {
      const directoryPath = path.resolve(options.dir).replace(/\\/g, "/");
      const outputPath = path.resolve(options.output).replace(/\\/g, "/");

      // Check if the changed file is within the directoryPath
      if (file.startsWith(directoryPath) && file !== outputPath) {
        console.log(`File changed: ${file}. Regenerating types...`);
        await generateFileTypes(
          directoryPath,
          outputPath,
          options.typeName || "Files"
        );
      }
    },
  };
}
