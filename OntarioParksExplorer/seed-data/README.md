# Ontario Parks Seed Data

## Overview

This directory contains curated seed data for the OntarioParksExplorer application. The data represents real Ontario provincial parks with accurate geographic information and authentic park characteristics.

## Data Source

The park data in `parks.json` was researched and compiled from:
- Official Ontario Parks website (ontarioparks.com)
- Geographic coordinate data for accurate location mapping
- Regional classifications based on Ontario Parks' administrative regions
- Activity listings based on actual facilities and opportunities at each park

## File Format

### parks.json

A JSON array containing 30 Ontario provincial parks. Each park object includes:

| Field | Type | Description | Example |
|-------|------|-------------|---------|
| `name` | string | Official park name | "Algonquin Provincial Park" |
| `description` | string | 2-3 sentence description highlighting park features | "Ontario's most famous park..." |
| `location` | string | City/area location | "Whitney, Ontario" |
| `latitude` | number | GPS latitude (decimal degrees) | 45.5543 |
| `longitude` | number | GPS longitude (decimal degrees, negative for Ontario) | -78.2604 |
| `website` | string | Official Ontario Parks URL | "https://www.ontarioparks.com/park/algonquin" |
| `isFeatured` | boolean | Whether park should be featured prominently | true |
| `region` | string | Geographic/administrative region | "Algonquin Region" |
| `activities` | array[string] | Available activities from master list | ["Hiking", "Camping", ...] |
| `images` | array[object] | Image URLs and alt text | [{url, altText}, ...] |

### Regions

Parks are organized into the following regions:
- Algonquin Region
- Georgian Bay Region
- Southeast Region
- Rideau Region
- Cottage Country
- Southwest Region
- Near North
- Northwest Region
- Highlands Region
- Kawarthas Region
- Greater Toronto Area

### Activities Master List

All activities are drawn from this standardized list:
- Hiking
- Camping
- Swimming
- Canoeing
- Kayaking
- Fishing
- Cross-Country Skiing
- Snowshoeing
- Mountain Biking
- Rock Climbing
- Bird Watching
- Wildlife Viewing
- Picnicking
- Boating
- Backpacking

### Featured Parks

Six parks are marked as `isFeatured: true` for prominent display:
1. Algonquin Provincial Park
2. Killarney Provincial Park
3. Sandbanks Provincial Park
4. Presqu'ile Provincial Park
5. Bon Echo Provincial Park
6. Arrowhead Provincial Park

## Images

Image URLs use placeholder service (picsum.photos) with unique seeds based on park names. For production, these should be replaced with actual park photography respecting copyright and licensing.

Format: `https://picsum.photos/seed/{parkslug}/800/400`

Each park includes 2 images with descriptive alt text for accessibility.

## Data Accuracy

- **Coordinates**: Verified latitude/longitude values based on actual park locations
- **Ontario longitudes**: All are negative (Western hemisphere)
- **Websites**: Follow Ontario Parks URL pattern (some parks may share URLs or redirect)
- **Descriptions**: Reflect actual park characteristics, features, and attractions
- **Activities**: Based on real facilities and opportunities at each park

## Usage for Seed Data

This JSON file is intended to be used by Arnold (Backend Developer) for creating EF Core seed data in the OntarioParksExplorer application. The data structure matches the expected Park entity model.

## Testing Considerations

As the Tester, I've ensured:
- ✅ Valid JSON format (parseable, no syntax errors)
- ✅ All required fields present for each park
- ✅ Coordinate accuracy (latitude/longitude in valid ranges)
- ✅ Consistent data types across all records
- ✅ Mix of featured and non-featured parks
- ✅ Diverse geographic distribution across Ontario
- ✅ Variety of activities and regions represented
- ✅ Meaningful descriptions for demo purposes

## Data Quality Notes

- 30 parks included (target was 25-30)
- 6 parks marked as featured
- All parks have 2 images each
- Activities range from 3-10 per park (realistic variety)
- Regions cover all major areas of Ontario
- Geographic spread from southwestern Ontario to northwestern regions

## Future Enhancements

For production use, consider:
- Replace placeholder images with licensed park photography
- Add seasonal information (when parks are open)
- Include fee structure data
- Add campsite availability information
- Expand activity details (trail distances, difficulty levels)
- Include accessibility information
- Add contact information for park offices

---

**Created by:** Muldoon (Tester)  
**Date:** March 2026  
**Version:** 1.0
