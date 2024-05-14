from flask import Flask, request, jsonify, send_file , redirect , render_template
from PIL import Image, ImageDraw
import math
import os
import time
# Create a Flask application
app = Flask(__name__)

# Load the base image


# Define a route for the /playerinfo endpoint
@app.route('/playerinfo', methods=['POST'])
async def update_player_info():

    base_image_path = './WebServerCS2/static/500x500.png'
    base_image = Image.open(base_image_path)

    if os.path.exists('./WebServerCS2/static/overlay_image.png'):
        os.remove('./WebServerCS2/static/overlay_image.png')

    
    
    # Check if the request contains JSON data
    if request.is_json:
        # Extract JSON data from the request
        data = request.get_json()

        if data is None:
            return jsonify({'error': 'No data provided'}), 400
        # Check if required data is available
        if 'team_num' in data and 'position' in data and 'map' in data:
            map = data['map']
            team_num = data['team_num']
            positions = data['position']
            

            if map == 'Dust II':
                offsetx = 2350 
                offsety = -3176 
            elif map == 'Mirage':
                print("oopies not done yet")
            
            for i in range(len(team_num)):
                #print(namelist[i] , team_num[i] , positions[i][0], positions[i][1])

                x = int(float(positions[i][0]))
                y = int(float(positions[i][1]))

                
                coordsx = round((offsetx + x)/8)
                coordsy = round((offsety + y)/8)

                if coordsy < 0:
                    coordsy = coordsy * -1

                if team_num[i] == 2:
                    draw_dot(base_image, coordsx, coordsy, (255, 255, 0))  # Red dot for team 2
                if team_num[i] == 3:
                    draw_dot(base_image, coordsx, coordsy, (0, 150, 255))  # Green dot for team 3


                base_image.save('./WebServerCS2/static/overlay_image.png')
            return jsonify({'message': 'Player information updated successfully.'}), 200
        else:
            return jsonify({'error': 'Missing parameters: team_num and/or position'}), 400
    else:
        return jsonify({'error': 'Request must be JSON'}), 400

# Define a route for the /map endpoint
@app.route('/map', methods=['GET'])
async def show_map():
    return render_template("map.html")

def draw_dot(image, x, y, color):
    draw = ImageDraw.Draw(image)
    draw.ellipse([x - 5, y - 5, x + 5, y + 5], fill=color)

# Run the application
if __name__ == '__main__':
    app.run(debug=True) 