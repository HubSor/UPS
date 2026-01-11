import http from 'k6/http';

export const options = {
    vus: 50,
    iterations: 500,
};

const URL_1 = "https://localhost:2443";
const URL_2 = "https://localhost:2444";
const URLS = [ URL_1, URL_2 ];
let iteration = 0;

export default function () {
    iteration++;

    const loginHeaders = {
        headers: {
            "Content-Type": "application/json",
        }
    }

    const url = URLS[iteration % 2];

    const loginPayload = JSON.stringify({
        username: "admin",
        password: "admin",
    });

    const loginRes = http.post(url + "/users/login", loginPayload, loginHeaders);
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
    const saveSaleRes = http.post(url + "/sales/save", saveSalePayload, salesHeaders);
    if (saveSaleRes.status != 200){
        console.error("save sale failed", saveSaleRes)
        throw new Error()
    }

    const listSalesPayload = JSON.stringify(
        { "pagination": { "pageSize": 10, "pageIndex": 0, "totalCount": 0, "totalPages": 1, "count": 0 } }
    )
    const listSalesRes = http.post(url + "/sales/list", listSalesPayload, salesHeaders);
    if (listSalesRes.status != 200){
        console.error("list sales failed", listSalesRes)
        throw new Error()
    }

    const getSale = JSON.stringify(
        { "saleId": saveSaleRes.json().data.saleId }
    )

    const getSaleRes = http.post(url + "/sales/get", getSale, salesHeaders);
    if (getSaleRes.status != 200){
        console.error("get sales failed", getSaleRes)
        throw new Error()
    }
}
