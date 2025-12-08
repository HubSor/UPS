import http from 'k6/http';

export const options = {
    vus: 1,
    iterations: 2000,
};

const URL = "https://localhost:2106";
let saleId = 1;

export default function () {
    const loginHeaders = {
        headers: {
            "Content-Type": "application/json",
        }
    }

    const loginPayload = JSON.stringify({
        username: "admin",
        password: "admin",
    });

    const loginRes = http.post(URL + "/users/login", loginPayload, loginHeaders);
    if (loginRes.status != 200){
        console.error("login failed", loginRes)
        throw new Error()
    }

    const cookie = loginRes.cookies["UPSAuth"][0].value;

    const salesHeaders = {
        headers: {
            "Cookie": `UPSAuth=${cookie}`,
            "Content-Type": "application/json",
        }
    };

    const saveSalePayload = JSON.stringify({
        "productId": 2,
        "subProducts": [
            { "subProductId": 1, "price": 9.99 },
            { "subProductId": 3, "price": 9.99 }
        ],
        "clientId": 5,
        "productPrice": 99.99,
        "answers": [
            { "parameterId": 4, "answer": "Warszawa" },
            { "parameterId": 5, "answer": "2" },
            { "parameterId": 9, "answer": "Niedziela" },
            { "parameterId": 6, "answer": "2.5" },
            { "parameterId": 1, "answer": "Test" },
            { "parameterId": 8, "answer": "Test" }
        ]
    });
    const saveSaleRes = http.post(URL + "/sales/save", saveSalePayload, salesHeaders);
    if (saveSaleRes.status != 200){
        console.error("save sale failed", saveSaleRes)
        throw new Error()
    }

    const listSalesPayload = JSON.stringify(
        { "pagination": { "pageSize": 10, "pageIndex": 0, "totalCount": 0, "totalPages": 1, "count": 0 } }
    )
    const listSalesRes = http.post(URL + "/sales/list", listSalesPayload, salesHeaders);
    if (listSalesRes.status != 200){
        console.error("list sales failed", listSalesRes)
        throw new Error()
    }

    const getSale = JSON.stringify(
        { "saleId": saleId++ }
    )
    const getSaleRes = http.post(URL + "/sales/get", getSale, salesHeaders);
    if (getSaleRes.status != 200){
        console.error("get sales failed", getSaleRes)
        throw new Error()
    }
}
