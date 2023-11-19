import { Component } from '@angular/core';
import { Subject, throwError } from 'rxjs';
import { catchError, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { Suggestion } from 'src/models/suggestion';
import { LyricService } from '../lyric.service';
import { ArtistEntry } from 'src/models/artistEntry';
import { ArtistSongs } from '../../models/artistSongs';
import { SongDetails } from '../../models/songDetails';
import { SongLyric } from '../../models/songLyric';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent {
  artists: Suggestion = { artists: [] };
  selectedArtist: any;
  selectedArtistId = '';
  isLoading = false;
  isCountingWords = false;
  isErrored = false;
  error = '';
  private searchTerms = new Subject<string>();
  songs: SongDetails[] = [];
  averageWordCount = 0;

  constructor(private lyricService: LyricService) {
    this.searchTerms.pipe(
      debounceTime(400),
      distinctUntilChanged(),
    ).subscribe(term => {
      if (term) {
        this.isLoading = true;
        this.artists = { artists: [] };
        this.lyricService.searchArtist(term).subscribe(artists => {
          this.isLoading = false;
          this.artists = artists;
        });
      }
    });
  }

  public isTypePersonOrBand(type: string): boolean {
    return type === 'Person' || type === 'Group';
  }

  public onArtistSearch(term: string): void {
    this.searchTerms.next(term);
  }

  public onGotoArtistDetail(artist: any): void {
    this.isLoading = true;
    this.searchTerms.next('');
    this.artists = { artists: [] };

    this.lyricService.processArtist(artist)
      .pipe(
        catchError(error => {
          this.isLoading = false;
          return throwError(() => new Error(error));
        })
      )
      .subscribe((data: ArtistSongs )=> {
        this.isLoading = false;
        this.songs = data.songs;
        this.selectedArtist = data.artist;
        this.selectedArtistId = data.artistId;
      });

  }

  public onCountWords(): void {
    this.isCountingWords = true;
    this.lyricService.countWords(this.selectedArtist, this.selectedArtistId)
      .subscribe((data: SongLyric[]) => {
        this.isCountingWords = false;
        this.averageWordCount = data.reduce((acc, val) => acc + val.wordCount, 0) / data.length;
      });
  }

  public onShowMeMore(): void {
    this.lyricService.navigateToDetails(this.selectedArtist, this.selectedArtistId);
  }

  ngOnDestroy(): void {
    this.searchTerms.unsubscribe();
  }
}
