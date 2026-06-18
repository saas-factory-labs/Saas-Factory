const fs = require('fs');
const path = require('path');

// Read batches
const batchesPath = path.join(__dirname, 'Code', '.understand-anything', 'intermediate', 'batches.json');
const batchesData = JSON.parse(fs.readFileSync(batchesPath, 'utf8'));
const batches = batchesData.batches;

console.log(`Processing ${batches.length} batches...`);

// Create a summary for the user to review before dispatching agents
const totalFiles = batches.reduce((sum, b) => sum + b.files.length, 0);
const codeFiles = batches.reduce((sum, b) => sum + b.files.filter(f => f.language).length, 0);

console.log(`Total files across all batches: ${totalFiles}`);
console.log(`Code files (with language info): ${codeFiles}`);

// Group batches into chunks of 5 for concurrent dispatch
const batchSize = 5;
const groups = [];
for (let i = 0; i < batches.length; i += batchSize) {
  groups.push(batches.slice(i, i + batchSize));
}

console.log(`Split into ${groups.length} dispatch groups`);

// Write group manifest for reference
const manifestPath = path.join(__dirname, 'Code', '.understand-anything', 'intermediate', 'batch-manifest.json');
fs.writeFileSync(manifestPath, JSON.stringify({
  totalBatches: batches.length,
  totalFiles,
  codeFiles,
  groups: groups.map((g, i) => ({
    groupIndex: i + 1,
    batchCount: g.length,
    files: g.reduce((sum, b) => sum + b.files.length, 0),
    batches: g.map(b => ({ index: b.batchIndex, fileCount: b.files.length }))
  }))
}, null, 2));

console.log(`\nBatch manifest written to ${manifestPath}`);
console.log('\nTo process these batches:');
groups.forEach((g, i) => {
  console.log(`Group ${i + 1}: batches [${g.map(b => b.batchIndex).join(', ')}]`);
});
