import { Review } from "./review.interface";

export interface Service {
    name: string;
    provider: string;
    description: string;
    contact: string;
    review?: Review;
    image: string;
    rating: number;
    price: number;
    workingHours: string;
    location: string;
}
  