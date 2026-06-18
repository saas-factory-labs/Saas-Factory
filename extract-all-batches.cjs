const fs = require('fs');
const path = require('path');

var bPath = 'Code/.understand-anything/intermediate/batches.json';
var bData = JSON.parse(fs.readFileSync(bPath, 'utf8'));
var allBatches = bData.batches;

var completed = [1,2,5,8,9,10,12,13,14,15,16,36];
var remainingIndices = [];
for (var i = 1; i <= allBatches.length; i++) {
    if (completed.indexOf(i) === -1) {
        remainingIndices.push(i);
    }
}

console.log('Need to extract + analyze ' + remainingIndices.length + ' batches');

var chunkSize = 5;
var groups = [];
for (var i = 0; i < remainingIndices.length; i += chunkSize) {
    groups.push(remainingIndices.slice(i, i + chunkSize));
}

console.log('Split into ' + groups.length + ' groups');

var tmpDir = 'Code/.understand-anything/tmp';
if (!fs.existsSync(tmpDir)) fs.mkdirSync(tmpDir, { recursive: true });

var pluginRoot = path.resolve('Code/AppBlueprint');

groups.forEach(function(group, gi) {
    var extractInputs = [];

    group.forEach(function(idx) {
        var batch = allBatches[idx - 1];
        if (!batch) return;

        batch.files.forEach(function(f) {
            var entry = {
                path: f.path,
                language: f.language || 'csharp',
                sizeLines: f.sizeLines || 0,
                fileCategory: f.fileCategory || 'code'
            };

            if (batch.batchImportData && batch.batchImportData.length > 0) {
                var imp = batch.batchImportData.find(function(b) { return b.filePath === entry.path; });
                if (imp) entry.batchImportData = imp.imports || [];
            }
            extractInputs.push(entry);
        });
    });

    var extractInput = {
        projectRoot: pluginRoot,
        batchFiles: extractInputs.map(function(ei) {
            return { path: ei.path, language: ei.language, sizeLines: ei.sizeLines, fileCategory: ei.fileCategory };
        }),
        batchImportData: extractInputs.map(function(ei) {
            return { filePath: ei.path, imports: ei.batchImportData || [] };
        })
    };

    fs.writeFileSync(
        path.join(tmpDir, 'extract-group-' + (gi + 1) + '.json'),
        JSON.stringify(extractInput)
    );

    console.log('Group ' + (gi + 1) + ': batches [' + group.join(',') + '] - ' + extractInputs.length + ' files');
});

console.log('\nExtraction input files written.');
