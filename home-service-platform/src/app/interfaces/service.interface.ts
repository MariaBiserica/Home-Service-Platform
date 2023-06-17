import { Review } from "./review.interface";

export interface Service {
    name: string;
    provider: string;
    description: string;
    review?: Review;
    image: string;
    rating: number;
}
  