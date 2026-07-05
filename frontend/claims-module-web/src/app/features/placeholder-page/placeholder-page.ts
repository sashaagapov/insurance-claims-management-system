import { AsyncPipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { ActivatedRoute } from '@angular/router';
import { map } from 'rxjs';

@Component({
  selector: 'app-placeholder-page',
  imports: [AsyncPipe, MatCardModule],
  templateUrl: './placeholder-page.html',
  styleUrl: './placeholder-page.scss',
})
export class PlaceholderPage {
  private readonly route = inject(ActivatedRoute);

  readonly page$ = this.route.data.pipe(
    map((data) => ({
      title: data['title'] as string,
      description: data['description'] as string,
    })),
  );
}
