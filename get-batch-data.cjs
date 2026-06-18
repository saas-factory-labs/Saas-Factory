const fs = require('fs');
const b = JSON.parse(fs.readFileSync('Code/.understand-anything/intermediate/batches.json', 'utf8'));

console.log('Total batches:', b.batches.length);

// Already completed batch files on disk
const completed = new Set([1,2,5,8,9,10,12,13,14,15,16,36]);

// Batches that still need processing
const remaining = [];
for (let i = 1; i <= b.batches.length; i++) {
    if (!completed.has(i)) {
        const batchData = b.batches[i-1];
        const fileCount = batchData.files.length;
        const hasImports = batchData.batchImportData && batchData.batchImportData.length > 0;
        remaining.push({ index: i, fileCount, hasImports });
    }
}

console.log('Remaining batches to process:', remaining.length);
remaining.forEach(r => {
    const filesStr = r.files ? r.files.map(f => `${f.path.split('/').pop()}(${f.language})`).join(', ') : '(see below)';
    console.log(`  Batch ${String(r.index).padStart(2)}: ${r.fileCount} files${r.hasImports ? ' [HAS IMPORTS]' : ''}`);
});

// Also show the file paths for each batch (first 3 per batch)
console.log('\nFile samples per batch:');
remaining.forEach(r => {
    const bd = b.batches[r.index - 1];
    console.log(`\n  Batch ${r.index}:`);
    bd.files.slice(0, 5).forEach(f => {
        console.log(`    ${f.path} (${f.language}, ${f.sizeLines} lines)`);
    });
    if (bd.files.length > 5) console.log(`    ... and ${bd.files.length - 5} more`);
});

// Write remaining batch details to a file for use in dispatches
const output = {
    totalBatches: b.batches.length,
    completed,
    remaining
};
fs.writeFileSync('Code/.understand-anything/tmp/remaining-batches.json', JSON.stringify(output, null, 2));
