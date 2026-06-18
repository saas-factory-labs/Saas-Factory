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

const outputDir = path.join(__dirname, 'Code', '.understand-anything', 'tmp');
if (!fs.existsSync(outputDir)) fs.mkdirSync(outputDir, { recursive: true });

// Project context for all agents
const projectContext = `Project: SaaS Factory - AppBlueprint & DeploymentManager — B2B/B2C SaaS blueprint using .NET 10, Aspire, Clean Architecture with DDD. Languages: csharp, razor, html, css, sql`;
const languageDirective = '> **Language directive**: Generate all textual content (summaries, descriptions, tags, titles, languageNotes, languageLesson) in **en**. Maintain technical accuracy while using natural, native-level phrasing in the target language. Keep technical terms in English when no standard translation exists.';

groups.forEach((group, gi) => {
  const dispatchData = {
    groupIndex: gi + 1,
    totalGroups: groups.length,
    projectContext,
    batches: group.map(b => ({
      batchIndex: b.batchIndex,
      files: b.files,
      batchImportData: b.batchImportData || [],
      neighborMap: b.neighborMap || {}
    }))
  };
  fs.writeFileSync(
    path.join(outputDir, `dispatch-group-${gi + 1}.json`),
    JSON.stringify(dispatchData, null, 2)
  );
});

console.log(`Prepared ${groups.length} dispatch groups in Code/.understand-anything/tmp/dispatch-group-{1..${groups.length}}.json`);
groups.forEach((g, i) => {
  console.log(`  Group ${i+1}: batches [${g.map(b=>b.batchIndex).join(',')}] - ${g.reduce((s,b)=>s+b.files.length,0)} files`);
});
