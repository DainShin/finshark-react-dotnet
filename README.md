# Backend
1. create a new project for .net 

    dotnet new webapi -o api

2.
    dotnet watch run     

3.
    dotnet ef migrations add init


# Claim
1. ClaimsPrincipal vs Claims
    1) ClaimsPrincipal
        - 사용자를 나타내는 객체
        - 사용자의 여러 "클레임을 포함"
        - ex. John Doe라는 사용자의 이름, 역할, 이메일 정보
    2) Claims
        - 사용자의 특정 속성
        - 각 클레임은 key-value 쌍으로 저장
        - ex. Name="John Doe", Role="Admin"    

2. ClaimsExtensions
    클레임 데이터에 쉽게 접근할 수 있는 메서드 제공
    ex.  사용자의 이름을 가져오기 위해 GetUsername() 이라는 메서드 만들어둔 것