import { useEffect, useState } from "react"
import { SalePathFormProps } from "../../pages/SaleMainPage"
import { ProductDto, ProductStatusEnum, ResultPaginationDto } from "../../api/Dtos"
import { PaginationBar, defaultPagination } from "../../helpers/FormHelpers"
import { Api } from "../../api/Api"
import { toastError } from "../../helpers/ToastHelpers"

type ChooseProductProps = SalePathFormProps

export const ChooseProductForm = ({ state, dispatch }: ChooseProductProps) => {
    const [products, setProducts] = useState<ProductDto[]>([]);
    const [pagination, setPagination] = useState<ResultPaginationDto>(defaultPagination);

    useEffect(() => {
        Api.ListProducts({ statuses: [ ProductStatusEnum.Offered ], pagination: pagination }).then(res => {
            if (res.data && res.success)
                setProducts(res.data.products.items)
            else toastError('Nie udało się pobrać oferowanych produktów')
        })
    }, [pagination])

    return <div>
        <h3>Wybierz produkt</h3>
        <br/>
        <table className="table table-striped col-lg-3">
            <thead>
                <tr className="table-dark">
                    <th scope="col">Kod</th>
                    <th scope="col">Nazwa</th>
                    <th scope="col">Podstawowa cena</th>
                </tr>
            </thead>
            <tbody>
                {products.map(p => {
                    return <tr key={p.id} onClick={() => {
                        dispatch({ type: 'setProduct', productId: p.id })
                    }} className={p.id === state.productId ? "table-primary" : ""}>
                        <td>
                            {p.code}
                        </td>
                        <td>
                            {p.name}
                        </td>
                        <td>
                            {p.basePrice}
                        </td>
                    </tr>
                })}
            </tbody>
        </table>
        <PaginationBar currentIndex={pagination.pageIndex} maxIndex={pagination.totalPages - 1}
            onNext={next => setPagination({ ...pagination, pageIndex: next})}
            onPrev={prev => setPagination({ ...pagination, pageIndex: prev })}
        />
    </div>
}