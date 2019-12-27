using MathematicalEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicRender.Engine {
    public class Camera {

        public enum Camera_Movement {
            FORWARD,
            BACKWARD,
            LEFT,
            RIGHT
        }

        private const float ONEDEGRAD = (float)(Math.PI / 180.0);
        private const float YAW = -90.0f;
        private const float PITCH = 0.0f;
        private const float SPEED = 10.0f;
        private const float SENSITIVTY = 0.1f;
        private const float ZOOM = 90.0f;

        private Vec3f Position;
        private Vec3f Front;
        private Vec3f Up;
        private Vec3f Right;
        private Vec3f WorldUp;

        private float Yaw;
        private float Pitch;

        private float MovementSpeed;
        private float MouseSensitivity;
        private float Zoom;

        public Camera(Vec3f position, Vec3f up, float yaw = YAW, float pitch = PITCH) {

            Front = new Vec3f(0.0f, 0.0f, -1.0f);
            MovementSpeed = SPEED;
            MouseSensitivity = SENSITIVTY;
            Zoom = ZOOM;
            Position = position;
            WorldUp = up;
            Yaw = yaw;
            Pitch = pitch;
            updateCameraVectors();
        }



        public Camera(float posX, float posY, float posZ, float upX, float upY, float upZ, float yaw, float pitch) {

            Front = new Vec3f(0.0f, 0.0f, -1.0f);
            MovementSpeed = SPEED;
            MouseSensitivity = SENSITIVTY;
            Zoom = ZOOM;
            Position = new Vec3f(posX, posY, posZ);
            WorldUp = new Vec3f(upX, upY, upZ);
            Yaw = yaw;
            Pitch = pitch;
            updateCameraVectors();
        }



        public Mat4f GetViewMatrix() {

            return Mat4f.lookAtRH(Position, Position + Front, Up);
        }

        public void ProcessKeyboard(Camera_Movement direction, float deltaTime) {

            float velocity = MovementSpeed * deltaTime;

            if (direction == Camera_Movement.FORWARD)
                Position += Front * velocity;

            if (direction == Camera_Movement.BACKWARD)
                Position -= Front * velocity;

            if (direction == Camera_Movement.LEFT)
                Position -= Right * velocity;

            if (direction == Camera_Movement.RIGHT)
                Position += Right * velocity;
        }

        public void ProcessKeyboard(Vec3f direction, float deltaTime) {

            float velocity = MovementSpeed * deltaTime;
            Position += direction * velocity;
        }

        public void ProcessMouseMovement(float xoffset, float yoffset, bool constrainPitch = true) {

            xoffset *= MouseSensitivity;
            yoffset *= MouseSensitivity;

            Yaw += xoffset;
            Pitch += yoffset;

            if (constrainPitch) {
                if (Pitch > 89.0f)
                    Pitch = 89.0f;

                if (Pitch < -89.0f)
                    Pitch = -89.0f;
            }

            updateCameraVectors();
        }



        public void ProcessMouseScroll(float yoffset) {

            if (Zoom >= 1.0f && Zoom <= 45.0f)
                Zoom -= yoffset;

            if (Zoom <= 1.0f)
                Zoom = 1.0f;

            if (Zoom >= 45.0f)
                Zoom = 45.0f;
        }

        public void updateCameraVectors() {

            Vec3f front = new Vec3f();

            front.x = (float)(Math.Cos(Yaw * ONEDEGRAD) * Math.Cos(Pitch * ONEDEGRAD));
            front.y = (float)(Math.Sin(Pitch * ONEDEGRAD));
            front.z = (float)(Math.Sin(Yaw * ONEDEGRAD) * Math.Cos(Pitch * ONEDEGRAD));
            Front = front.unit();

            Right = (Front * WorldUp).normal();
            Up = (Right * Front).normal();
        }
    }
}
