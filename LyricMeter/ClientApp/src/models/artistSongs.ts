import { SongDetails } from "./songDetails";

export interface ArtistSongs {
  artist: string;
  artistId: string;
  created: Date;
  songs: SongDetails[];
}
