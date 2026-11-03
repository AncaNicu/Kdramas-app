import { KdramaSummary } from "./kdrama-summary";

export interface ActorDetails
{
    actorId: number;
    name: string;
    birthDate: string | null;
    imageUrl: string | null;
    kdramas: KdramaSummary[];
}