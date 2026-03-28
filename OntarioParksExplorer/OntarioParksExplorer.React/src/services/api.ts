import axios, { AxiosError } from 'axios';
import { 
  ParkListDto, 
  ParkDetailDto, 
  ActivityDto, 
  PagedResult,
  RecommendationRequest,
  ParkRecommendation,
  ChatMessage,
  VisitPlanRequest,
  VisitPlan
} from '../types';

const apiClient = axios.create({
  baseURL: '/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

export class ApiError extends Error {
  constructor(public statusCode: number, message: string) {
    super(message);
    this.name = 'ApiError';
  }
}

const handleError = (error: unknown): never => {
  if (axios.isAxiosError(error)) {
    const axiosError = error as AxiosError;
    const statusCode = axiosError.response?.status || 500;
    const message = axiosError.response?.data 
      ? (typeof axiosError.response.data === 'string' 
          ? axiosError.response.data 
          : JSON.stringify(axiosError.response.data))
      : axiosError.message;
    throw new ApiError(statusCode, message);
  }
  throw new Error('An unexpected error occurred');
};

export const getParks = async (page: number = 1, pageSize: number = 12): Promise<PagedResult<ParkListDto>> => {
  try {
    const response = await apiClient.get<PagedResult<ParkListDto>>('/parks', {
      params: { page, pageSize }
    });
    return response.data;
  } catch (error) {
    return handleError(error);
  }
};

export const getParkById = async (id: number): Promise<ParkDetailDto> => {
  try {
    const response = await apiClient.get<ParkDetailDto>(`/parks/${id}`);
    return response.data;
  } catch (error) {
    return handleError(error);
  }
};

export const searchParks = async (query: string, page: number = 1, pageSize: number = 12): Promise<PagedResult<ParkListDto>> => {
  try {
    const response = await apiClient.get<PagedResult<ParkListDto>>('/parks/search', {
      params: { q: query, page, pageSize }
    });
    return response.data;
  } catch (error) {
    return handleError(error);
  }
};

export const filterParks = async (
  activities: string[], 
  mode: 'any' | 'all' = 'any', 
  page: number = 1, 
  pageSize: number = 12
): Promise<PagedResult<ParkListDto>> => {
  try {
    const response = await apiClient.get<PagedResult<ParkListDto>>('/parks/filter', {
      params: { 
        activities: activities.join(','), 
        mode, 
        page, 
        pageSize 
      }
    });
    return response.data;
  } catch (error) {
    return handleError(error);
  }
};

export const getActivities = async (): Promise<ActivityDto[]> => {
  try {
    const response = await apiClient.get<ActivityDto[]>('/activities');
    return response.data;
  } catch (error) {
    return handleError(error);
  }
};

// AI API endpoints
export const generateSummary = async (parkId: number): Promise<string> => {
  try {
    const response = await apiClient.post<string>(`/ai/parks/${parkId}/summary`);
    return response.data;
  } catch (error) {
    return handleError(error);
  }
};

export const getRecommendations = async (request: RecommendationRequest): Promise<ParkRecommendation[]> => {
  try {
    const response = await apiClient.post<ParkRecommendation[]>('/ai/recommendations', request);
    return response.data;
  } catch (error) {
    return handleError(error);
  }
};

export const chat = async (message: string, history: ChatMessage[]): Promise<string> => {
  try {
    const response = await apiClient.post<string>('/ai/chat', {
      message,
      conversationHistory: history
    });
    return response.data;
  } catch (error) {
    return handleError(error);
  }
};

export const planVisit = async (request: VisitPlanRequest): Promise<VisitPlan> => {
  try {
    const response = await apiClient.post<VisitPlan>('/ai/plan-visit', request);
    return response.data;
  } catch (error) {
    return handleError(error);
  }
};
