# Decisions

## Decision: Journey Documentation as First-Class Artifact

**Author:** Malcolm (Lead/Architect)  
**Date:** 2026-04-11  
**Status:** Implemented

### Context

The project needed a narrative document (`docs/JOURNEY.md`) that tells the story of how the Squad team built OntarioParksExplorer. This document targets developers evaluating AI-assisted development workflows, conference presentations, and blog post material.

### Decision

Created `docs/JOURNEY.md` as a 7-chapter narrative organized around the git commit timeline. The document emphasizes the OpenAI→Copilot SDK pivot as the key proof point (adapting to scope change > greenfield scaffolding). Includes 4 embedded screenshots, architectural decisions excerpts, and honest discussion of what didn't work perfectly.

### Rationale

- Journey documentation makes the development process reproducible and auditable
- The commit history is the ground truth — the narrative follows it exactly
- Honesty about limitations (timing bugs, CORS workarounds) builds credibility with developer audiences
- The decisions trail section demonstrates that AI agents reason about trade-offs, not just generate code

### Impact

- `docs/JOURNEY.md` added to documentation suite alongside User Manual and Demo Script
- No code changes — documentation only

---

## Decision: LifeLog AI PRD Structure as Reusable Template

**Author:** Malcolm (Lead/Architect)  
**Date:** 2026-03-29  
**Status:** Accepted

### Context

The `docs/lifelog_ai_prompt.md` skeleton was insufficient for Squad to produce a correct build on first pass. The OntarioParksExplorer experience proved that AI teams drift from specs — especially on AI provider choice — unless constraints are stated explicitly and redundantly.

### Decision

The enriched LifeLog AI PRD establishes a template pattern for future Squad PRDs:

1. **Meta-instruction header** — tells Squad to create team + plan before implementing
2. **Stack table with warnings** — critical constraints called out with ⚠️ markers
3. **Architecture constraints** — non-negotiable patterns that prevent known failure modes
4. **Data model with seed data** — prevents "Test Entry 1" quality issues
5. **Lessons learned section** — explicit warnings about pitfalls from prior builds
6. **Three-phase execution** — plan → implement → polish with human review gates

### Key Constraint

AI provider is specified THREE times (stack table, architecture section, lessons learned) because the #1 lesson from OntarioParksExplorer is that AI teams default to OpenAI unless repeatedly told otherwise.

### Impact

Future PRDs for Squad should follow this structure. The redundancy is intentional — it's cheaper to repeat yourself in a PRD than to rework a completed implementation.

---

## Decision: Documentation Narrative Standards

**Author:** Malcolm  
**Date:** 2026-04-11  
**Status:** Accepted

### Context

Rewrote `docs/JOURNEY.md` to fix a fundamental narrative error. The v1 incorrectly framed the OpenAI→Copilot SDK change as a "pivot" when the original PRD explicitly specified Copilot SDK from the start. This was spec drift followed by human correction, not a scope change.

### Decision

**Documentation narrative standard:** When documenting project history, always verify claims against primary sources (PRD, git commits, decisions.md). Quote original specs verbatim when claims depend on them.

**For JOURNEY.md specifically:**
- Show the FULL original PRD, not a paraphrased one-liner
- Frame the OpenAI→Copilot SDK change accurately: **the team drifted from spec, the human enforced it**
- This makes the story MORE compelling: proves AI teams need human oversight
- Emphasize how architectural decisions (abstraction layers) enabled painless correction

### Rationale

**Why this matters:**
- The TRUE story is more interesting than the fabricated one
- "AI team drifts from spec, human catches it" proves the value of human product management
- Documenting drift honestly builds trust in the workflow
- Shows that good architecture (abstractions) pays off even when you build the wrong thing initially

**What changed in v2:**
1. Chapter 1: Full PRD quoted verbatim (40 lines) with meta-instruction emphasized
2. Chapter 3: Explicitly states Grant used OpenAI when spec said Copilot SDK — drift identified
3. Chapter 4: Renamed from "The Pivot" to "The Human Catches the Drift" — accurate framing
4. Chapter 6: Decisions reframed as enablers of the correction (not just good practices)
5. Takeaway: "The prompt is the product spec. The human is the product manager. And sometimes QA too."

### Impact

- Corrects historical record for future reference
- Establishes precedent: documentation must be honest about failures and drift
- Proves the workflow value: AI builds fast, humans enforce specs, architecture makes fixes cheap
- Makes the Squad workflow MORE credible, not less (honesty about limitations)
