var Entity = function (spawnX, spawnY, type) {
    
    this.type = type;
    this.x = spawnX;
    this.y = spawnY;

    this.vx = 0;
    this.vy = 0;

    this.id = globalSpawnId;

    this.speed = 0;
    this.maxSpeed = 8;

    this.targetX = Math.random() * MAP_WIDTH;
    this.targetY = Math.random() * MAP_HEIGHT;

    if (this.type == "human") {
        this.color = "blue";
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

        var nextSpot = map.getImageData(parseInt(xp), parseInt(yp), 1, 1);
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

        // next spot is NOT clear; stop here if player
        } else if (this.type == "human"){
            this.x = parseInt(startx);
            this.y = parseInt(starty);
            this.targetX = this.x;
            this.targetY = this.y;
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

            var collisionDistance = (othersX - x0) * (othersX - x0) + (othersY - y0) * (othersY - y0);
            collisionDistance = Math.sqrt(collisionDistance);

            // check for collisions
            if (collisionDistance < COLLISION_DISTANCE) {
                console.log("bump!  " + this.id + " with " + others[i].id);
                if (this.type == "human" || others[i].type == "human") {
                    gameOver = true;
                    player.type = "bimbo";
                    player.color = "red";
                    enemies.push(player);
                }
                else {
                    this.x = this.x + (Math.random() * 10 - 5);
                    this.y = this.y + (Math.random() * 10 - 5);
                    
                    this.targetX = Math.random() * MAP_WIDTH;
                    this.targetY = Math.random() * MAP_HEIGHT;
                }
                
            }

        }
    }

};

Entity.prototype.draw = function (ctx) {
    ctx.beginPath();
    ctx.arc(this.x, this.y, 10, 0, Math.PI * 2, true);
    ctx.closePath();
    ctx.fillStyle = this.color;
    ctx.fill();
};

