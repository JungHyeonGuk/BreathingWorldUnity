# 레퍼런스
Play on website: https://breathingworld.com

Reference web-client github: https://github.com/Farer/breathingworld_client_web


# 개요
해당 유니티 프로젝트는 오픈소스입니다.

자유롭게 수정하실 수 있습니다.

아래는 AI의 도움을 받은 ImageFx와 Aseprite로 그린 픽셀아트 이미지입니다.

![Nature](https://github.com/user-attachments/assets/c4912832-06c8-4d2d-9d2b-e3051799a5ba)
![Rabbit](https://github.com/user-attachments/assets/e988f76f-c223-4304-aa7e-2bad1cfec4c1)
![Wolf](https://github.com/user-attachments/assets/91daac92-2acf-4de1-9090-8858868984ff)


# 개발자 설명
잡초는 6단계, 나무는 11단계가 존재합니다.

전체 맵의 크기는 1920x1080이며 한 픽셀이 1m 단위입니다.

서버에서 전달해주는 값은 왼쪽 상단이 (0, 0) 이라서 유니티 좌표계의 경우 1079 - y 값을 해줘야 월드 좌표로 변환됩니다.

구역은 48x27크기라서 왼쪽위 districtId가 0이고 그 옆이 1이고, 0의 아래는 40입니다.

그래서 구역의 사이즈는 40x40개 입니다.

동물은 위치 단위가 16이 더 곱해져있기에 16을 나눠야합니다.







