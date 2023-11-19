import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, concatMap, from, map, mergeMap, range, startWith, toArray } from 'rxjs';
import { Suggestion } from 'src/models/suggestion';
import { ArtistEntry } from 'src/models/artistEntry';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { ArtistSongs } from '../models/artistSongs';
import { SongLyric } from '../models/songLyric';

@Injectable({
  providedIn: 'root'
})
export class LyricService {
  private readonly api = environment.apiBaseUrl;
  private readonly suggest = `${this.api}Artist/GetArtistName/`;
  private readonly lyrics = `${this.api}Artist/GetAllLyricsByArtist/`;
  private readonly recordings = `${this.api}Artist/GetAllSongsByArtist/`;

  private readonly artistStorageKey = 'artist:';
  private readonly detailsRoute = '/details/';

  constructor(private http: HttpClient, private router: Router) { }

  public searchArtist(term: string): Observable<Suggestion> {
    return this.http.get<Suggestion>(`${this.suggest}${term}`);
  }

  public processArtist(artist: any): Observable<ArtistSongs> {
    return this.http.get<any>(`${this.recordings}${artist.name}/${artist.id}`);
  }

  public countWords(name: string, artistId: string): Observable<SongLyric[]> {
    return this.http.get<SongLyric[]>(`${this.lyrics}${name}/${artistId}`);
  }

  public navigateToDetails(artist: string, artistId: string): void {
    this.router.navigate([this.detailsRoute, artist, artistId]);
  }

}
