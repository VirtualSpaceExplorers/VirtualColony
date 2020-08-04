/*
  Create rows of tension columns for the 570 hab.
  (Quick and dirty version)
  
  OpenSCAD source code.
  
  File units are meters
*/
OD=0.3;
ht=12;
run=155; // meters down the rows of mattress columns
count=12;
spacing=run/count; // distance between columns

fan_len=5; // top fans out this far below the top
nfan=4; // number of fanned supports
fan=45; fan_spacing=45/(nfan/2);
fOD=OD/sqrt(nfan);

bollard=0.5; // vehicle bollard
bollard_ht=3;

module positioning() {
    rotate([0,0,90]) // go down Y axis
    translate([-run/2+spacing/2,0,5.5]) // centeraxis
       children();
}

module foreach_column() {
    positioning()
    for (x=[0:spacing:run-spacing/2])
        translate([x,0,0])
            children();
}

module supports() {
    // Top solid part
    positioning()
    translate([-spacing/2,-OD/2,ht-OD/2]) cube([run,OD,OD]);

    foreach_column()
    {
        translate([-OD/2,-OD/2,0])
            cube([OD,OD,ht]);
        translate([0,0,ht-fan_len])
        for (angle=[-fan:fan_spacing:+fan])
            if (angle!=0) rotate([0,angle,0])
                translate([-fOD/2,-fOD/2,0])
                cube([fOD,fOD,fan_len/cos(angle)]);
    }
}

module bollards() {
    foreach_column()
        translate([-bollard/2,-bollard/2,0])
            cube([bollard,bollard,bollard_ht]);
}

//bollards();
supports();

