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
