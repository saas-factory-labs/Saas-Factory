const fs = require('fs');
const path = require('path');

const batchesPath = path.join(__dirname, 'Code', '.understand-anything', 'intermediate', 'batches.json');
const data = JSON.parse(fs.readFileSync(batchesPath, 'utf8'));
const batches = data.batches;

// Group into chunks of 5 for concurrent dispatch
const batchSize = 5;
const groups = [];
for (let i = 0; i < batches.length; i += batchSize) {
  groups.push(batches.slice(i, i + batchSize));
}

// Write a dispatch script that creates file-analyzer prompts
console.log(`Total batches: ${batches.length}`);
console.log(`Dispatch groups needed: ${groups.length} (5 batches each)`);

// For efficiency, write each group as a separate dispatch manifest
const outputDir = path.join(__dirname, 'Code', '.understand-anything', 'tmp');
if (!fs.existsSync(outputDir)) fs.mkdirSync(outputDir, { recursive: true });

for (let g = 0; g < groups.length; g++) {
  const groupBatches = groups[g];
  const manifest = {
    groupIndex: g + 1,
    totalGroups: groups.length,
    batches: groupBatches.map(b => ({
      batchIndex: b.batchIndex,
      files: b.files.map(f => ({
        path: f.path,
        language: f.language,
        sizeLines: f.sizeLines,
        fileCategory: f.fileCategory
      })),
      batchImportData: b.batchImportData || [],
      neighborMap: b.neighborMap || {}
    }))
  };
  fs.writeFileSync(
    path.join(outputDir, `dispatch-group-${g + 1}.json`),
    JSON.stringify(manifest, null, 2)
  );
}

console.log(`\nDispatch manifests written to Code/.understand-anything/tmp/dispatch-group-{1..${groups.length}}.json`);
console.log('\nTo process: Dispatch file-analyzer agents for each group concurrently.');
