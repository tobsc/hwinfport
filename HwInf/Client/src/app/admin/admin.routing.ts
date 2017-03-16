import {Routes, RouterModule} from "@angular/router";
import { AuthGuard } from "../authentication/auth.guard";
import { AdminGuard } from "../authentication/admin.guard";
import {AdminDashboardComponent} from "./admin-dashboard/admin-dashboard.component";
import {HomeComponent} from "../home/home.component";
import {DeviceListComponent} from "../admin/devices/device-list/device-list.component";
import {DeviceAddComponent} from "./devices/device-add/device-add.component";
import {DeviceTypesComponent} from "./devices/device-types/device-types.component";
const ADMIN_ROUTES: Routes = [
    { path: 'admin', component: HomeComponent, canActivate: [AuthGuard],
        children: [
            { path: '', redirectTo: 'dashboard', pathMatch: 'full', },
            { path: 'dashboard', component: AdminDashboardComponent, canActivate: [AuthGuard] },
            { path: 'geraete', component: DeviceListComponent, canActivate: [AdminGuard] },
            { path: 'geraete/page/:page', component: DeviceListComponent },
            { path: 'geraete/neu', component: DeviceAddComponent},
            { path: 'geraete/typen', component: DeviceTypesComponent }
        ]
    }
];
export const adminRouting = RouterModule.forChild(ADMIN_ROUTES);
