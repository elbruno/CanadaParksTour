export interface ParkListDto {
  id: number;
  name: string;
  region: string;
  isFeatured: boolean;
  latitude: number;
  longitude: number;
  mainImageUrl: string;
  activityNames: string[];
}

export interface ParkDetailDto {
  id: number;
  name: string;
  description: string;
  location: string;
  latitude: number;
  longitude: number;
  website: string;
  isFeatured: boolean;
  region: string;
  createdAt: string;
  updatedAt: string;
  activities: ActivityDto[];
  images: ParkImageDto[];
}

export interface ActivityDto {
  id: number;
  name: string;
}

export interface ParkImageDto {
  id: number;
  url: string;
  altText: string;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface RecommendationRequest {
  activities: string[];
  region?: string;
  preferenceText?: string;
}

export interface ParkRecommendation {
  parkId: number;
  parkName: string;
  reason: string;
}

export interface ChatMessage {
  role: string;
  content: string;
}

export interface VisitPlanRequest {
  parkId: number;
  durationDays: number;
  interests: string[];
  season?: string;
}

export interface DayItinerary {
  day: number;
  title: string;
  activities: string[];
  tips: string[];
}

export interface VisitPlan {
  parkName: string;
  overview: string;
  itinerary: DayItinerary[];
  packingList: string[];
  tips: string[];
}
