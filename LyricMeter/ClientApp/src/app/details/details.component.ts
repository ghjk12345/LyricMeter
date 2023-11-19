import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { LyricService } from '../lyric.service';
import * as Highcharts from 'highcharts';
import { SongLyric } from '../../models/songLyric';
import { finalize } from 'rxjs';


@Component({
  selector: 'app-details',
  templateUrl: './details.component.html',
  styleUrls: ['./details.component.scss']
})
export class DetailsComponent {
  artist: string = '';
  error = false;
  id: string = '';
  artistData: SongLyric[] = [];
  songsBarChart: typeof Highcharts = Highcharts;
  songsBarChartOptions: Highcharts.Options = this.createChartOption();

  calculatedHeight = '600px';

  constructor(private route: ActivatedRoute, private lyricService: LyricService) {}

  ngOnInit(): void {
    this.artist = this.route.snapshot.paramMap.get('artist') ?? '';
    this.id = this.route.snapshot.paramMap.get('id') ?? '';
    if (this.artist && this.id) {
      this.lyricService.countWords(this.artist, this.id)
        .pipe(
          finalize(() => {
            this.songsBarChartOptions = this.createChartOption();
          })
        )
        .subscribe(data => {
          if (!data || !data.length) {
            this.error = true;
            throw new Error('No data returned');
          }
          this.artistData = data;
        });
    }
  }

  private createChartOption(): Highcharts.Options {
    this.calculatedHeight = this.artistData.length > 30 ? `${this.artistData.length * 20}px` : '600px';
    const sorted = this.artistData.sort((a, b) => b.wordCount - a.wordCount);

    const categories = sorted.map(entry => entry.title);
    const data = sorted.map(entry => entry.wordCount);

    return {
      chart: {
        type: 'bar',
        events: {
          render: function () {
            this.reflow()
          }
        }
      },
      title: {
        text: `Word Count in Songs by ${this.artist}`,
        align: 'left'
      },
      xAxis: {
        categories: categories, 
        title: {
          text: 'Songs'
        },
        gridLineWidth: 1,
        lineWidth: 0
      },
      yAxis: {
        min: 0,
        title: {
          text: 'Word Count',
          align: 'high'
        },
        labels: {
          overflow: 'justify'
        },
        gridLineWidth: 0
      },
      tooltip: {
        valueSuffix: ' words'
      },
      plotOptions: {
        bar: {
          dataLabels: {
            enabled: true
          }
        }
      },
      credits: {
        enabled: false
      },
      series: [{
        name: 'Word Count',
        type: 'bar', 
        data: data
    }]
    };
  }

}
