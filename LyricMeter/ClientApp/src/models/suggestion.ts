export interface Suggestion {
    artists: Artists[];
}

export interface Artists {
    id: string;
    name: string;
    type: string;
    country?: string;
}