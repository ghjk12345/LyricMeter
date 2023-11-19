export interface Counted {
    artist: string;
    data: SongResult[];
}

export interface SongResult {
    song: string;
    count: number;
    isSuccessful: boolean;
}