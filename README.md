# AssetReferencesTool
Unity 프로젝트에서 특정 에셋(Sprite, Prefab, Material 등)이 어디서 참조되고 있는지 찾아주는 에디터 확장 툴 입니다.  

### 사용 방법
1. Unity 메뉴에서 Tools > AssetReferenceFinder를 클릭하면 창이 열립니다.
2. Target Asset 필드에 참조를 찾고 싶은 에셋을 드래그 앤 드롭합니다.
3. 검색할 파일 타입을 토글로 선택합니다.
4. Find References 버튼을 클릭하면 검색이 시작됩니다.
5. 검색이 끝나면 결과 목록이 창에 표시되고, 각 항목을 클릭하면 해당 파일이 프로젝트 창에서 하이라이트 됩니다.
6. 결과는 텍스트 파일로 저장되며, 저장 위치는 Result Save Path 필드에서 변경할 수 있습니다.

### 스크린샷
<img width="490" height="262" alt="스크린샷 2026-07-03 120201" src="https://github.com/user-attachments/assets/840fa209-be0f-445a-8fda-c57e62a02b5f" /> <img width="611" height="218" alt="스크린샷 2026-07-03 120208" src="https://github.com/user-attachments/assets/bf3f713c-be1c-4a0c-8062-ba1cd64cbc98" /> <img width="479" height="67" alt="스크린샷 2026-07-03 120216" src="https://github.com/user-attachments/assets/9adf2bd2-432c-4a50-ab5d-594ef770ff34" />


### 옵션
Filter by Sprite Type Only : 체크 시 단순 GUID 텍스트 매칭 대신, 프리팹/씬을 실제로 로드해서 SpriteRenderer 컴포넌트가 대상 스프라이트를 사용하는지 정밀 검사합니다.

### 주의사항
- Edit > Project Setting > Editor > Asset Serialization Mode = Force Text
- 프로젝트 규모가 큰 경우, 파일을 전부 읽어야 하므로 검색에 시간이 걸릴 수 있습니다.
- 코드로 동적 참조하는 경우, 감지하지 못합니다.

