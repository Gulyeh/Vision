import { ThisReceiver } from '@angular/compiler';
import { Component } from '@angular/core';
import { BusyService } from './_services/busy.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'Vision';
}
