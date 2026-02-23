#!/usr/bin/env node
// Parses the plain-text output of `npx azimutt analyze` and writes a
// formatted Markdown report to docs/azimutt-database-analysis-report.md

import * as fs from "fs";
import * as path from "path";

const inputFile: string = process.argv[2] ?? "azimutt-output.txt";
const outputFile: string =
  process.argv[3] ?? "docs/azimutt-database-analysis-report.md";

const raw: string = fs.readFileSync(inputFile, "utf8");
const lines: string[] = raw.split("\n");

// ── Helpers ────────────────────────────────────────────────────────────────

function toTitle(s: string): string {
  return s
    .replace(/_/g, " ")
    .replace(/\b\w/g, (c) => c.toUpperCase());
}

function today(): string {
  return new Date().toISOString().split("T")[0];
}

// ── Types ──────────────────────────────────────────────────────────────────

interface Rule {
  count: number;
  name: string;
  violations: string[];
}

type Severity = "high" | "medium" | "low" | "hint";

// ── Extract metadata ───────────────────────────────────────────────────────

const versionMatch = raw.match(/Version\s+(\S+)/);
const version: string = versionMatch ? versionMatch[1] : "unknown";

const hasPgStat: boolean = !raw.includes("pg_stat_statements is not enabled");

const entityMatch = raw.match(
  /Found (\d+) entities, (\d+) relations, (\d+) queries and (\d+) types/
);
const entities: string = entityMatch ? entityMatch[1] : "?";
const relations: string = entityMatch ? entityMatch[2] : "?";
const queries: string = entityMatch ? entityMatch[3] : "?";
const types: string = entityMatch ? entityMatch[4] : "?";

const summaryMatch = raw.match(
  /Found (\d+) violations using (\d+) rules: (\d+) high, (\d+) medium, (\d+) low, (\d+) hint/
);
const totalCount: number = summaryMatch ? parseInt(summaryMatch[1]) : 0;
const highCount: number = summaryMatch ? parseInt(summaryMatch[3]) : 0;
const mediumCount: number = summaryMatch ? parseInt(summaryMatch[4]) : 0;
const lowCount: number = summaryMatch ? parseInt(summaryMatch[5]) : 0;
const hintCount: number = summaryMatch ? parseInt(summaryMatch[6]) : 0;

// ── Line-by-line parser ────────────────────────────────────────────────────
// Produces: Map<Severity, Rule[]>

const SEVERITY_HEADER =
  /^(\d+) (hint|low|medium|high) violations \(\d+ rules\):/;
const RULE_LINE = /^  (\d+) (.+?):\s*$/;
const VIOLATION_LINE = /^    - (.+)$/;
const MORE_LINE = /^    \.\.\. \d+ more/;

const sections = new Map<Severity, Rule[]>([
  ["high", []],
  ["medium", []],
  ["low", []],
  ["hint", []],
]);

let currentSeverity: Severity | null = null;
let currentRule: Rule | null = null;

for (const line of lines) {
  const sev = line.match(SEVERITY_HEADER);
  if (sev) {
    currentSeverity = sev[2] as Severity;
    currentRule = null;
    continue;
  }

  if (!currentSeverity) continue;

  const rule = line.match(RULE_LINE);
  if (rule) {
    currentRule = {
      count: parseInt(rule[1]),
      name: rule[2].trim(),
      violations: [],
    };
    sections.get(currentSeverity)!.push(currentRule);
    continue;
  }

  const violation = line.match(VIOLATION_LINE);
  if (violation && currentRule) {
    currentRule.violations.push(violation[1].trim());
    continue;
  }

  // "... N more" continuation – keep as a note
  if (MORE_LINE.test(line) && currentRule) {
    const m = line.match(/\.\.\. (\d+) more/);
    if (m) currentRule.violations.push(`_+ ${m[1]} more_`);
  }
}

// ── Markdown builders ──────────────────────────────────────────────────────

function buildSection(
  severity: string,
  emoji: string,
  count: number,
  rules: Rule[] | undefined
): string {
  if (!rules || rules.length === 0) return "";

  const active = rules.filter((r) => r.count > 0);
  let md = `## ${emoji} ${severity.charAt(0).toUpperCase() + severity.slice(1)} Severity (${count} violation${count !== 1 ? "s" : ""})\n\n`;

  if (active.length === 0) {
    md += "_No violations in this category._\n\n---\n\n";
    return md;
  }

  for (const rule of active) {
    md += `### ${toTitle(rule.name)} — ${rule.count} violation${rule.count !== 1 ? "s" : ""}\n\n`;
    for (const v of rule.violations) {
      md += `- ${v}\n`;
    }
    md += "\n";
  }

  md += "---\n\n";
  return md;
}

// ── Assemble report ────────────────────────────────────────────────────────

let report = `# 🔍 Azimutt Database Analysis Report

> **Database:** \`appblueprintdb\`
> **Report Date:** ${today()}
> **Azimutt CLI Version:** ${version}
> **Tool:** [Azimutt](https://azimutt.app/) — Next-Gen ERD & Database Linter
> **Workflow:** [View latest run](https://github.com/saas-factory-labs/Saas-Factory/actions/workflows/azimutt-database-analysis.yml)

---

## Summary

| Severity | Count |
|----------|------:|
| 🔴 High   | ${highCount} |
| 🟠 Medium | ${mediumCount} |
| 🔵 Low    | ${lowCount} |
| 💡 Hint   | ${hintCount} |
| **Total** | **${totalCount}** |

> Scanned **${entities} entities**, **${relations} relations**, **${queries} queries**, **${types} types**.

---

`;

report += buildSection("high", "🔴", highCount, sections.get("high"));
report += buildSection("medium", "🟠", mediumCount, sections.get("medium"));
report += buildSection("low", "🔵", lowCount, sections.get("low"));
report += buildSection("hint", "💡", hintCount, sections.get("hint"));

report += `## ℹ️ Notes\n\n`;

if (!hasPgStat) {
  report += `- \`pg_stat_statements\` is **not enabled**. Enabling it on the PostgreSQL instance will allow Azimutt to analyse slow & degrading queries.\n`;
}
report += `- To receive the full report as JSON, pass \`--email your@email.com\` to the CLI.\n`;
report += `\n---\n\n_Generated automatically by [Azimutt CLI](https://www.npmjs.com/package/azimutt) · [Re-run analysis](https://github.com/saas-factory-labs/Saas-Factory/actions/workflows/azimutt-database-analysis.yml)_\n`;

// ── Write output ───────────────────────────────────────────────────────────

fs.mkdirSync(path.dirname(outputFile), { recursive: true });
fs.writeFileSync(outputFile, report, "utf8");
console.log(`Report written to ${outputFile}`);
