var Entity = function (spawnX, spawnY, type, control) {
    
    this.type = type;
    this.x = spawnX;
    this.y = spawnY;

    this.vx = 0;
    this.vy = 0;

    this.id = globalSpawnId;

    this.speed = 0;

    if (type == "human") {
        this.maxSpeed = 8;
    } else if (type == "bimbo") {
        this.maxSpeed = 6;
    }
    
    this.control = control;

    this.targetX = Math.random() * MAP_WIDTH;
    this.targetY = Math.random() * MAP_HEIGHT;

    if (this.type == "human") {
        if (this.control == "player") {
            this.color = "blue";
        } else {
            this.color = "lightblue";
        }
        
    } else {
        this.color = "pink";
    }

    globalSpawnId++;

    this.stunTimer = 0;
  
};

Entity.prototype.setTarget = function (targetX, targetY) {
    this.targetX = targetX;
    this.targetY = targetY;
}

// methods
Entity.prototype.moveTo = function (others, map) {


    var startx = this.x;
    var starty = this.y;

    var x0 = this.x;
    var y0 = this.y;

    if (this.control=="player") {
   // console.log(x0 + "," + y0);

    }
    
    var x1 = this.targetX;
    var y1 = this.targetY;

    

    var m = 99999;

    try {
        m = (y1 - y0) / (x1 - x0);
    } catch (err) {
       
    }

    var distance = (x1 - x0) * (x1 - x0) + (y1 - y0) * (y1 - y0);
    distance = Math.sqrt(distance);

    // arrived!
    if (distance < this.speed)
    {
        this.x = x1;
        this.y = y1;

        this.speed = .01;
        

    }

    // still moving
    else
    {
        

        var percentAlongLine = this.speed / distance;

        var xp = (y1 - y0) * percentAlongLine / m + x0;
        var yp = m * (x1 - x0) * percentAlongLine + y0;

        var nextSpot = ctx_fullmap.getImageData(parseInt(xp), parseInt(yp), 1, 1);
        //if (this.type == "human") {
        //    console.log(nextSpot.data[2]);
        //}

        // next spot is clear
        if (nextSpot.data[0] == 255) {
            this.x = xp;
            this.y = yp;

            // not there yet, speed up
            if (this.speed < this.maxSpeed) {
                this.speed += ACCELERATION;
            }

        // next spot is NOT clear; stop here if human
        } else if (this.type == "human"){
            this.x = parseInt(startx);
            this.y = parseInt(starty);
            this.targetX = this.x;
            this.targetY = this.y;

       // next spot is NOT clear; bounce off if bimbo
        } else if (this.type == "bimbo") {
            this.x = parseInt(startx);
            this.y = parseInt(starty);
            this.targetX = -this.targetX;
            this.targetY = -this.targetY;
        }

        

    }


    // check to see if there's a collision with anything else.
    for (var i = 0; i < others.length; i++) {

        // self, don't worry about a collision
        if (others[i].id == this.id) {
            continue;
        } else {

            var othersX = others[i].x;
            var othersY = others[i].y;

            var distance = (othersX - x0) * (othersX - x0) + (othersY - y0) * (othersY - y0);
            distance = Math.sqrt(distance);

            // distance is within collision threshhold; do something
            if (distance < COLLISION_DISTANCE) {

                // gameover for human character
                if (this.type == "bimbo" && others[i].type == "human") {
                    
                    others[i].type = "bimbo";
                    

                    if (others[i].control == "player") {
                        gameOver = true;
                        enemies.push(player);
                        others[i].color = "red";
                    } else {
                        others[i].color = "pink";
                    }
                    
                }

                // two bots collided... do... something?
                else {
                   // this.x = this.x + (Math.random() * 10 - 5);
                    //this.y = this.y + (Math.random() * 10 - 5);
                    
                    //this.targetX = Math.random() * MAP_WIDTH;
                    //this.targetY = Math.random() * MAP_HEIGHT;
                }
                
            }

            // this is a bimbo; aggro on the target
            else if (this.type == "bimbo" && distance < AGGRO_DISTANCE && others[i].type == "human") {
                this.targetX = others[i].x;
                this.targetY = others[i].y;
            }

        }
    }

};

Entity.prototype.draw = function (ctx) {
    ctx.beginPath();
    ctx.arc(this.x - mapOffsetX, this.y - mapOffsetY, 10, 0, Math.PI * 2, true);
    ctx.closePath();
    ctx.fillStyle = this.color;
    ctx.fill();
};

function spawnEntities(count, type) {

    var breakout = 0;
    var realspawns = 0;

    while (breakout < 100 && realspawns < count) {
        var x = Math.floor(Math.random() * WORLD_WIDTH);
        var y = Math.floor(Math.random() * WORLD_HEIGHT);

        var nextSpot = ctx_fullmap.getImageData(x, y, 1, 1);

        if (nextSpot.data[0] == 255) {
            var spawn = new Entity(x, y, type, "ai");
            allEntities.push(spawn);
            enemies.push(spawn);
            realspawns++;
        } else {
            breakout++;
        }

       

    }
}