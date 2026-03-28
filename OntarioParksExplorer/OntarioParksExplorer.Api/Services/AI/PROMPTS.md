# AI Prompt Templates Documentation

This document describes the AI prompt templates used in the OntarioParksExplorer AI service layer.

## Overview

All prompts are designed with three key principles:
1. **Token Efficiency** — Concise, clear instructions with minimal redundancy
2. **Consistent Formatting** — Predictable output structure for reliable parsing
3. **Graceful Degradation** — Fallback responses when AI service is unavailable

## Prompt Templates

### 1. Park Summary

**Purpose**: Generate engaging 2-3 sentence summaries for park detail pages.

**Location**: `PromptTemplates.ParkSummary()`

**Inputs**:
- `parkName` — Name of the park
- `description` — Brief park description from database
- `activities` — List of available activities (limited to 10 for token efficiency)

**Output Format**: Plain text (2-3 sentences)

**Sample Output**:
```
Algonquin Provincial Park is Ontario's most iconic wilderness destination, featuring over 7,600 square 
kilometers of pristine forests, lakes, and rivers. Perfect for canoeing, hiking, and wildlife viewing, 
with opportunities to see moose, bears, and loons in their natural habitat.
```

**Caching**: Results cached for 24 hours using `park-summary-{parkId}` as cache key.

---

### 2. Park Recommendations

**Purpose**: Provide personalized park recommendations based on user preferences.

**Location**: `PromptTemplates.Recommendations()`

**Inputs**:
- `activities` — List of preferred activities
- `region` — Optional region filter (e.g., "Northern Ontario")
- `preferenceText` — Optional free-text preferences

**Output Format**: JSON
```json
{
  "recommendations": [
    {
      "parkName": "Killarney Provincial Park",
      "reason": "Perfect for hiking and canoeing with stunning white quartzite ridges.",
      "matchScore": 0.95
    }
  ]
}
```

**Match Scores**: 0.0-1.0 scale, clamped in code for safety.

**Notes**:
- AI returns park names which are matched against database
- Unknown parks are filtered out with debug logging
- Limited to top 5 recommendations

---

### 3. Chat / Q&A

**Purpose**: Answer user questions about Ontario parks conversationally.

**Location**: `PromptTemplates.Chat()`

**Inputs**:
- `question` — User's question
- `parkContext` — Minimal, relevant park data (context-aware filtering)

**Output Format**: Plain text response

**Context Optimization**:
- Only loads 5 parks maximum
- Filters by region keywords in question (north, south, east, west)
- Truncates descriptions to 100 characters
- Limits activities to 5 per park
- Conversation history limited to last 10 messages

**Sample Interaction**:
```
Q: What parks in Northern Ontario are good for fishing?
A: Northern Ontario has excellent fishing parks. Lake Superior Provincial Park offers world-class 
fishing for lake trout and salmon. Quetico Provincial Park is a paddler's paradise with remote 
fishing opportunities for walleye, northern pike, and smallmouth bass.
```

---

### 4. Visit Plan

**Purpose**: Generate multi-day itineraries tailored to visitor interests.

**Location**: `PromptTemplates.VisitPlan()`

**Inputs**:
- `parkName` — Name of the park
- `parkDescription` — Brief park description
- `activities` — Available activities at the park
- `durationDays` — Number of days (1-14, validated by API)
- `interests` — Visitor's interests
- `season` — Optional season context

**Output Format**: JSON
```json
{
  "days": [
    {
      "dayNumber": 1,
      "title": "Arrival and Lake Exploration",
      "activities": ["Canoeing", "Swimming", "Campsite Setup"],
      "description": "Arrive early to set up camp, then spend the afternoon..."
    }
  ],
  "tips": [
    "Book campsites in advance during peak season",
    "Bring insect repellent"
  ]
}
```

**Fallback Plan**:
- Empty days array with informative tips
- Recommends checking park's official website

---

## Error Handling

All AI methods implement comprehensive error handling:

1. **AI Not Configured**: Returns friendly messages indicating configuration needed
2. **Timeout (30 seconds)**: Cancellation token with specific timeout messages
3. **JSON Parse Errors**: Logged with fallback to empty/default responses
4. **General Exceptions**: Logged with appropriate fallback content

## Configuration

AI service requires configuration in `appsettings.json`:

```json
{
  "AI": {
    "ApiKey": "your-api-key-here",
    "Model": "gpt-4o-mini",
    "Provider": "OpenAI"
  }
}
```

When `ApiKey` is not set, all methods return graceful fallback messages.

## Performance Optimizations

1. **Caching**: Park summaries cached for 24 hours
2. **Context Limiting**: Chat loads minimal data based on question
3. **Timeouts**: 30-second timeout prevents hanging requests
4. **Activity Limiting**: Max 10 activities per prompt
5. **Conversation History**: Limited to last 10 messages
6. **JSON Options**: Pre-configured with case-insensitive parsing

## Testing Without AI

When developing without an AI API key:
- All endpoints remain functional
- Return informative "AI not configured" messages
- Don't break user experience
- Allow testing of non-AI functionality
