---
name: testing-patterns
description: Testing patterns and principles for Java/Spring and Angular applications. Unit, integration, mocking strategies.
allowed-tools: Read, Write, Edit, Glob, Grep, Bash
---

# Testing Patterns

> Principles for reliable test suites in Java/Spring and Angular applications.

---

## 1. Testing Pyramid

```
        /\          E2E (Few)
       /  \         Critical user flows (Selenium, Cypress)
      /----\
     /      \       Integration (Some)
    /--------\      API endpoints, DB queries, Service layer
   /          \
  /------------\    Unit (Many)
                    Services, Utils, Components
```

---

## 2. AAA Pattern

| Step | Purpose | Example |
|------|---------|---------|
| **Arrange** | Set up test data | `Customer customer = new Customer("John", Status.ACTIVE);` |
| **Act** | Execute code under test | `CustomerResponse result = service.getById(1L);` |
| **Assert** | Verify outcome | `assertEquals("John", result.getName());` |

---

## 3. Test Type Selection

| Type | Best For | Speed | Tools |
|------|----------|-------|-------|
| **Unit** | Service logic, Utils, Validators | Fast (<50ms) | JUnit 5, Mockito / Jasmine, Karma |
| **Integration** | API endpoints, Repository queries | Medium | @SpringBootTest, TestRestTemplate |
| **E2E** | Critical user flows | Slow | Selenium, Cypress |

---

## 4. Java/Spring Unit Test Principles

### Good Unit Tests (FIRST)

| Principle | Meaning |
|-----------|---------|
| **Fast** | < 100ms each |
| **Isolated** | No external deps (DB, API) |
| **Repeatable** | Same result always |
| **Self-checking** | No manual verification |
| **Timely** | Written with code |

### What to Test

| ✅ Test | ❌ Don't Test |
|---------|--------------|
| Service business logic | Framework code (Spring internals) |
| Edge cases & boundary values | Simple getters/setters |
| Error handling & exceptions | Third-party library code |
| Validation logic | Entity constructors |

### Java Example

```java
@ExtendWith(MockitoExtension.class)
class CustomerServiceTest {

    @Mock
    private CustomerRepository customerRepository;

    @InjectMocks
    private CustomerServiceImpl customerService;

    @Test
    @DisplayName("Should return customer when valid ID provided")
    void getById_ValidId_ReturnsCustomer() {
        // Arrange
        CustomerEntity entity = CustomerEntity.builder()
            .id(1L).name("John").status(Status.ACTIVE).build();
        when(customerRepository.findById(1L)).thenReturn(Optional.of(entity));

        // Act
        CustomerResponse result = customerService.getById(1L);

        // Assert
        assertEquals("John", result.getName());
        assertEquals(Status.ACTIVE, result.getStatus());
    }

    @Test
    @DisplayName("Should throw exception when customer not found")
    void getById_InvalidId_ThrowsException() {
        when(customerRepository.findById(99L)).thenReturn(Optional.empty());

        assertThrows(ResourceNotFoundException.class,
            () -> customerService.getById(99L));
    }
}
```

---

## 5. Angular Unit Test Principles

### Component Testing

```typescript
describe('CustomerListComponent', () => {
    let component: CustomerListComponent;
    let fixture: ComponentFixture<CustomerListComponent>;
    let customerService: jasmine.SpyObj<CustomerService>;

    beforeEach(async () => {
        const spy = jasmine.createSpyObj('CustomerService', ['getAll', 'delete']);

        await TestBed.configureTestingModule({
            declarations: [CustomerListComponent],
            providers: [{ provide: CustomerService, useValue: spy }]
        }).compileComponents();

        fixture = TestBed.createComponent(CustomerListComponent);
        component = fixture.componentInstance;
        customerService = TestBed.inject(CustomerService) as jasmine.SpyObj<CustomerService>;
    });

    it('should load customers on init', () => {
        const mockData = [{ id: 1, name: 'John', status: 'ACTIVE' }];
        customerService.getAll.and.returnValue(of(mockData));

        component.ngOnInit();

        expect(component.customers.length).toBe(1);
        expect(component.customers[0].name).toBe('John');
    });
});
```

### Service Testing

```typescript
describe('CustomerService', () => {
    let service: CustomerService;
    let httpMock: HttpTestingController;

    beforeEach(() => {
        TestBed.configureTestingModule({
            imports: [HttpClientTestingModule],
            providers: [CustomerService]
        });
        service = TestBed.inject(CustomerService);
        httpMock = TestBed.inject(HttpTestingController);
    });

    it('should fetch customers', () => {
        service.getAll().subscribe(data => {
            expect(data.length).toBe(1);
        });

        const req = httpMock.expectOne('/api/customers');
        expect(req.request.method).toBe('GET');
        req.flush([{ id: 1, name: 'John' }]);
    });
});
```

---

## 6. .NET/C# Unit Test Principles (xUnit + Moq)

### Service Testing

```csharp
public class CustomerServiceTests
{
    private readonly Mock<ICustomerRepository> _mockRepository;
    private readonly CustomerService _service;

    public CustomerServiceTests()
    {
        _mockRepository = new Mock<ICustomerRepository>();
        _service = new CustomerService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetById_ValidId_ReturnsCustomer()
    {
        // Arrange
        var entity = new CustomerEntity { Id = 1, Name = "John", Status = Status.Active };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.Equal("John", result.Name);
        Assert.Equal(Status.Active, result.Status);
    }

    [Fact]
    public async Task GetById_InvalidId_ThrowsNotFoundException()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((CustomerEntity?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(99));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public async Task Create_InvalidName_ThrowsValidationException(string? name)
    {
        var request = new CreateCustomerRequest { Name = name };

        await Assert.ThrowsAsync<ValidationException>(() => _service.CreateAsync(request));
    }
}
```

### Controller Testing

```csharp
public class CustomerControllerTests
{
    private readonly Mock<ICustomerService> _mockService;
    private readonly CustomerController _controller;

    public CustomerControllerTests()
    {
        _mockService = new Mock<ICustomerService>();
        _controller = new CustomerController(_mockService.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithList()
    {
        // Arrange
        var customers = new List<CustomerDto>
        {
            new() { Id = 1, Name = "John" },
            new() { Id = 2, Name = "Jane" }
        };
        _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(customers);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var data = Assert.IsAssignableFrom<IEnumerable<CustomerDto>>(okResult.Value);
        Assert.Equal(2, data.Count());
    }
}
```

---

## 7. Mocking Principles

### When to Mock

| ✅ Mock | ❌ Don't Mock |
|---------|--------------|
| Repository/DAO layer | The service under test |
| External APIs (REST clients) | Simple domain objects |
| Time/Date (Clock) | Pure utility functions |
| Message queues | In-memory data structures |

### Mock Types

| Type | Java (Mockito) | .NET (Moq) | Angular (Jasmine) |
|------|----------------|------------|-------------------|
| Stub | `when(...).thenReturn(...)` | `Setup(...).Returns(...)` | `spy.method.and.returnValue(...)` |
| Spy | `@Spy` | `mock.CallBase = true` | `jasmine.createSpyObj(...)` |
| Verify | `verify(mock).method()` | `mock.Verify(m => m.Method())` | `expect(spy.method).toHaveBeenCalled()` |

---

## 7. Integration Test (Java/Spring)

```java
@SpringBootTest(webEnvironment = SpringBootTest.WebEnvironment.RANDOM_PORT)
class CustomerControllerIntegrationTest {

    @Autowired
    private TestRestTemplate restTemplate;

    @Test
    void createCustomer_ValidRequest_Returns201() {
        CreateCustomerRequest request = new CreateCustomerRequest("John", "john@test.com");

        ResponseEntity<CustomerResponse> response = restTemplate.postForEntity(
            "/api/customers", request, CustomerResponse.class);

        assertEquals(HttpStatus.CREATED, response.getStatusCode());
        assertNotNull(response.getBody().getId());
    }
}
```

---

## 8. Test Naming Convention

| Pattern | Example |
|---------|---------|
| `methodName_StateUnderTest_ExpectedBehavior` | `getById_ValidId_ReturnsCustomer` |
| `should_ExpectedBehavior_When_StateUnderTest` | `should_ThrowException_When_CustomerNotFound` |

---

## 9. Anti-Patterns

| ❌ Don't | ✅ Do |
|----------|-------|
| Test implementation details | Test behavior/output |
| Duplicate test setup code | Use `@BeforeEach` / `beforeEach` |
| Write tests after code | Write tests alongside or before (TDD) |
| Ignore flaky tests | Fix root cause immediately |
| Skip cleanup | Reset state in `@AfterEach` |

---

> **Remember:** Tests are documentation. If someone can't understand what the code does from the tests, rewrite them.
